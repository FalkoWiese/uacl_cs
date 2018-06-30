using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using MultiClientConsole;
using Newtonsoft.Json;
using UaclClient;
using UaclServer;
using UaclUtils;
using UnifiedAutomation.UaClient;

namespace MultiClientConsole
{
    class WpcConstants
    {
        public static string ClientPrefix = "WEBER_MASCHINENBAU_GMBH.Systems.L00_SYSTEM.Modules";
        public static string TrackSuffix = "Statistics.WeigherStatisticsPlugin";
    }

    class WpcRemoteObject : RemoteObject
    {
        protected WpcRemoteObject(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
            AnnounceSessionNotConnectedHandler(DisconnectCallback);
            AnnouncePostConnectionEstablishedHandler(ReconnectCallback);
        }

        public ModuleDescription Desc { protected get; set; }

        private void DisconnectCallback(Session session, ServerConnectionStatusUpdateEventArgs args)
        {
            Logger.Info($"NotConnectedHandler called for {session} with status {session.ConnectionStatus.ToString()}.");
            if (session.ConnectionStatus != ServerConnectionStatus.Disconnected) return;
            Logger.Info($"The connection {session} is disconnected.");
            MonitoringStarted = false;
            StartConnectionEstablishment();
        }

        private void ReconnectCallback(Session session, ServerConnectionStatusUpdateEventArgs args)
        {
            Logger.Info($"PostConnectionHandler called for {session} with status {session.ConnectionStatus.ToString()}.");
            if (session.ConnectionStatus != ServerConnectionStatus.Connected) return;
            Logger.Info($"Connection {session} to ServerConsole established.");
            StartMonitoring();
        }

        private void StartMonitoring()
        {
            if (MonitoringStarted) return;
            MonitoringStarted = true;
            DoStartMonitoring();
        }

        protected virtual void DoStartMonitoring()
        {
        }

        private bool MonitoringStarted { get; set; }
    }

    [UaObject]
    class WpcTrackClient : WpcRemoteObject
    {
        private static string TRACK1 = "WEIGHER_TRACE_1";
        private static string TRACK2 = "WEIGHER_TRACE_2";
        private static string TRACK3 = "WEIGHER_TRACE_3";
        private static string TRACK4 = "WEIGHER_TRACE_4";
        private static string TRACK5 = "WEIGHER_TRACE_5";
        private static string TRACK6 = "WEIGHER_TRACE_6";

        public WpcTrackClient(string ip, int port, string name)
            : base(ip, port, $"{WpcConstants.ClientPrefix}.{name}.{WpcConstants.TrackSuffix}")
        {
        }

        private readonly Dictionary<string, string> Tracks = new Dictionary<string, string>
            {
                {TRACK1, "Track1"},
                {TRACK2, "Track2"},
                {TRACK3, "Track3"},
                {TRACK4, "Track4"},
                {TRACK5, "Track5"},
                {TRACK6, "Track6"},
            };



        protected override void DoStartMonitoring()
        {
            foreach (var trackName in Tracks.Keys)
            {
                var propertyName = Tracks[trackName];
                var property = GetType().GetProperty(propertyName);
                if (property == null)
                {
                    Logger.Warn($"Cannot find UaVariable for name ... {propertyName}!");
                    continue;
                }

                property.SetValue(this, Read<string>(trackName));

                Monitor(trackName, v =>
                {
                    property.SetValue(this, v.ToString());
                    var currentValue = property.GetValue(this).ToString();
                    Logger.Info($"Received value from {Name}.{trackName} ... '{currentValue}'.");
                    RestClientHelper.PostData(Desc, JsonHelper.SerializePackage(Desc, currentValue));
                });
            }
        }

        [UaMethod]
        public void PostData(int count)
        {
            var rand = new Random();
            Func<float> randomValue =
                () => float.Parse($"100.{rand.Next(0, 100)}", CultureInfo.InvariantCulture.NumberFormat);

            for (var i = 0; i < Math.Abs(count); i++)
            {
                int track = i%6 + 1;
                var measurementContent = new
                {
                    value = randomValue(),
                    idx = i,
                    trace = track,
                    load = i%50+1,
                };
                var property = GetType().GetProperty($"Track{track}");
                property.SetValue(this, measurementContent.value.ToString());
                RestClientHelper.PostData(Desc, JsonHelper.SerializePackage(Desc, JsonConvert.SerializeObject(measurementContent)));
            }
        }

        [UaVariable]
        public string Track1 { get; set; }

        [UaVariable]
        public string Track2 { get; set; }

        [UaVariable]
        public string Track3 { get; set; }

        [UaVariable]
        public string Track4 { get; set; }

        [UaVariable]
        public string Track5 { get; set; }

        [UaVariable]
        public string Track6 { get; set; }
    }

    [UaObject]
    class WpcModuleClient : WpcRemoteObject
    {
        private static string OMAC_STATE_NAME = "OMAC_STATE";
        private static string OMAC_MESSAGE_NAME = "OMAC_MESSAGE";

        public WpcModuleClient(string ip, int port, string name) : base(ip, port, $"{WpcConstants.ClientPrefix}.{name}")
        {
        }

        protected override void DoStartMonitoring()
        {
            State = Read<string>(OMAC_STATE_NAME);
            Message = Read<string>(OMAC_MESSAGE_NAME);

            Monitor(OMAC_STATE_NAME, v =>
            {
                State = v.ToString();
                Logger.Info($"Received value from {Name}.{OMAC_STATE_NAME} ... '{State}'.");
                RestClientHelper.PostData(Desc, JsonHelper.SerializeState(Desc, State));
            });

            Monitor(OMAC_MESSAGE_NAME, v =>
            {
                Message = v.ToString();
                Logger.Info($"Received value from {Name}.{OMAC_MESSAGE_NAME} ... '{Message}'.");
                var desc = Desc;
                var jsonString = JsonHelper.SerializeMessage(ref desc, Message);
                RestClientHelper.PostData(desc, jsonString);
            });
        }

        [UaMethod]
        public void SendViaMqtt()
        {
            RestClientHelper.SendViaMQTT(Desc, "100,GuidosDevice,Guido");
        }

        [UaMethod]
        public void PostData(int mode, int state, int subState)
        {
            var eventContent = new
            {
                mode = new
                {
                    id = mode,
                    name = JsonHelper.GetText(
                        () => Enum.GetName(typeof (OPERATION_MODE), (OPERATION_MODE) mode))
                },
                state = new
                {
                    id = state,
                    name = JsonHelper.GetText(
                        () => Enum.GetName(typeof (STATE_CODE), (STATE_CODE) state))
                },
                sub_state = new
                {
                    id = subState,
                    name = JsonHelper.GetText(
                        () => Enum.GetName(typeof (SUB_STATE_CODE), (SUB_STATE_CODE) subState))
                }
            };

            RestClientHelper.PostData(Desc, JsonHelper.SerializeState(Desc, JsonConvert.SerializeObject(eventContent)));
        }

        [UaVariable]
        public string State { get; set; }

        [UaVariable]
        public string Message { get; set; }
    }
}