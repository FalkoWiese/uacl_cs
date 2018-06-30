using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Enum;
using OpenNETCF.MQTT;

namespace UaclUtils
{
    public class AppDescription
    {
        public string BaseUrl { get; set; }
        public string DeviceId { get; set; }
    }

    public class ModuleDescription
    {
        public string UrlPrefix { get; set; }
        public string AdamosId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Suffix { get; set; }
        public string PostPackageSuffix { get; set; }

        public Uri GenerateUri()
        {
            return new Uri($"{UrlPrefix}/{Suffix}");
        }

        public CredentialCache Credentials()
        {
            var uri = GenerateUri();
            var cred = new NetworkCredential(Username, Password);
            return new CredentialCache {{uri, "Basic", cred}};
        }
    }

    public enum OPERATION_MODE
    {
        KEIN_MODE = 0,
        AUTOMATIK = 1,
        SERVICE = 2,
        ABGEWAEHLT = 3,
        REINIGUNG = 4,
        MANUELL = 5,
    };

    public enum STATE_CODE
    {
        SPANNUNGSFREI = 0,
        SPANNUNG_EIN = 1,
        INITIALISIERUNG = 2,
        ANLAGE_STEHT = 3,
        REFERENZFAHRT = 4,
        GRUNDSTELLUNGSFAHRT = 5,
        BETRIEBSBEREIT = 6,
        HOCHFAHREN = 7,
        BETRIEB = 8,
        MASCHINENSTOPP = 9,
        FEHLERSTOPP = 10,
        FEHLERSTOPP_FERTIG = 11,
        FEHLER_QUITTIEREN = 12,
        SYSTEME_RUNTERFAHREN = 13,
        KONFIGURATION_MASCHINE = 14,
        LEERFAHREN = 15,
        SPANNUNG_AUS = 16,
        TIMEOUT_KOMMUNIKATION = 17,
    };

    public enum SUB_STATE_CODE
    {
        KEIN_SUB_CODE = 0,
        ARBEITEN = 1, // Arbeiten
        PRODUKTWECHSEL = 2, // Produktwechsel
        WARTEN_AUF_NE = 3, // Warten auf nachgeschaltete Einheit (NE)
        WARTEN_AUF_VE = 4, // Warten auf vorgeschaltete Einheit (VE)
        WARTEN_AUF_BEDIENER = 5, // Warten auf Bediener (z. B. manuelle Beladung, Interleaverpapier nachfuellen, etc.)
        PAUSIEREN = 6, // Pausieren (z. B. ext. Leerschnitt)
        PRODUKTIONSUNTERBRECHUNG_WEGEN_STOERUNG = 7,
        // Produktionsunterbrechung auf Grund von Stoerung, Bedienereingriff erforderlich  (Zufuehrungslichtschranke verschmutzt, Fehler Scannertabelle)
    };

    public class UtcHelper
    {
        public static string Now()
        {
            return DateTime.UtcNow.ToString("o");
        }
    }

    public class JsonHelper
    {
        public static string GetText(Func<string> func)
        {
            try
            {
                var text = func();
                return text;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return "";
        }

        public static string SerializeState(ModuleDescription desc, string json)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            var eventContent = new
            {
                source = new {id = desc.AdamosId},
                type = "WeberStateEventType",
                text = "Raised State Changes",
                time = UtcHelper.Now(),
                mode = new
                {
                    id = jsonObj.mode,
                    name = JsonHelper.GetText(
                        () => Enum.GetName(typeof (OPERATION_MODE), (OPERATION_MODE) jsonObj.mode))
                },
                state = new
                {
                    id = jsonObj.code,
                    name = JsonHelper.GetText(
                        () => Enum.GetName(typeof (STATE_CODE), (STATE_CODE) jsonObj.code))
                },
                sub_state = new
                {
                    id = jsonObj.sub_code,
                    name = JsonHelper.GetText(
                        () => Enum.GetName(typeof (SUB_STATE_CODE), (SUB_STATE_CODE) jsonObj.sub_code))
                }
            };

            return JsonConvert.SerializeObject(eventContent);
        }

        public static string SerializeMessage(ref ModuleDescription desc, string json)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            dynamic content = null;

            if (jsonObj.category_name.ToString() != "ERROR")
            {
                desc.Suffix = "event/events";
                content = new
                {
                    source = new {id = desc.AdamosId},
                    type = "WeberMessageEventType",
                    text = "Raised Message Changes",
                    time = UtcHelper.Now(),
                    message = new
                    {
                        id = jsonObj.code,
                        sub_id = jsonObj.sub_code,
                        sub_sub_id = jsonObj.context_code,
                        name = jsonObj.name
                    },
                    category = new
                    {
                        id = jsonObj.category_id,
                        name = jsonObj.category_name
                    }
                };
            }
            else
            {
                desc.Suffix = "alarm/alarms";
                content = SerializeAlarm(ref desc, $"{jsonObj.code}.{jsonObj.sub_code} - '{jsonObj.name}'", "CRITICAL");
            }

            return JsonConvert.SerializeObject(content);
        }


        public static string SerializeAlarm(ref ModuleDescription desc, string textStr, string severityStr)
        {
            var content = new
            {
                source = new { id = desc.AdamosId },
                type = "WeberAlarmType",
                time = UtcHelper.Now(),
                text = textStr,
                severity = severityStr,
                status = "ACTIVE"
            };

            return JsonConvert.SerializeObject(content);
        }
        public static string SerializePackage(ModuleDescription desc, string json)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            var measurementContent = new
            {
                source = new {id = desc.AdamosId},
                type = "WeberPackageMeasurement",
                time = UtcHelper.Now(),
                WeberPackageMeasurement = new
                {
                    packageWeight = new {value = jsonObj.value, unit = "g"},
                    portionNumber = new {value = jsonObj.idx},
                    trackNumber = new {value = jsonObj.trace},
                    loadingNumber = new {value = jsonObj.load},
                }
            };

            return JsonConvert.SerializeObject(measurementContent);
        }
    }

    public enum PostProtocol
    {
        REST, MQTT
    }

    /// <summary>
    /// RestClientHelper - kapselt die Verwendung der Spring REST Client Library    /// 
    /// </summary>
    public static class RestClientHelper
    {
        public static void PostData(ModuleDescription desc, string data, PostProtocol protocol = PostProtocol.REST)
        {
            if (protocol == PostProtocol.REST)
            {
                Task.Run(() => SendViaREST(desc, data));
            }
            else
            {
                Task.Run(() => SendViaMQTT(desc, data));
            }
        }

        public static void SendViaMQTT(ModuleDescription desc, string data)
        {
            var client = new MQTTClient(desc.UrlPrefix, 1883);
            client.Connected += (sender, args) =>
            {
                Logger.Info($"MQTT Connected ... {sender}.{args}");
                client.Publish("s/us", data, QoS.FireAndForget, true);
            };
            client.MessageReceived += (topic, qos, payload) =>
            {
                Logger.Info($"MQTT Message recv {topic}, {qos}, {payload}");
            };
            client.Disconnected += (sender, args) =>
            {
                Logger.Info($"MQTT Disconected ... {sender}.{args}");   
            };
            client.Connect("123456", $"weber2/{desc.Username}", desc.Password);
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                var state = client.ConnectionState;
                Logger.Info($"MQTT ConectionState == {state}");
                if (state == ConnectionState.Connected)
                {
                    client.Publish("s/us",data, QoS.FireAndForget, true);
                }
            }
       }

        private static void SendViaREST(ModuleDescription desc, string data)
        {
            Logger.Info($"Try to POST data ...\n{data}");
            var byteData = Encoding.UTF8.GetBytes(data);
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            ServicePointManager.DefaultConnectionLimit = 4;
            ServicePointManager.MaxServicePointIdleTime = 500;
            var request = WebRequest.Create(desc.GenerateUri()) as HttpWebRequest;
            if (request == null)
            {
                Logger.Warn($"Cannot send data ...\n{data}\n... via WebRequest without errors!");
                return;
            }

            request.Credentials = desc.Credentials();
            request.KeepAlive = false;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 10000;
            request.ContentLength = byteData.Length;

            try
            {
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(byteData, 0, byteData.Length);
                }
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    request = null;
                    if (response != null && response.StatusCode == HttpStatusCode.Created)
                    {
                        var location = response.Headers["Location"];
                        if (location != null)
                        {
                            Logger.Info("Notified at: " + location);
                        }
                    }
                    Logger.Info(
                        $"Webrequest Status Code: {response.StatusCode}\nDescription: {response.StatusDescription}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }


    }
}