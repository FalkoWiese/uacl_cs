using System;
using System.Collections.Generic;
using UaclClient;
using UaclServer;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace MultiClientConsole
{
    [UaObject]
    class ClientConsoleClient : RemoteObject
    {
        private ClientConsoleClient(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
            AnnounceSessionNotConnectedHandler(
                (session, args) =>
                {
                    if (session.ConnectionStatus != ServerConnectionStatus.Connected)
                    {
                        // Here, you can execute some useful stuff to handle the connection, maybe you can call s.Disconnect()
                        // or something else. Test it, and give me maybe some feedback.
                        Logger.Warn(
                            $"The connection {session} is disconnected, you can find the reason maybe at the arguments ... {args}.");
                        MonitoringStarted = false;
                    }
                });
            AnnouncePostConnectionEstablishedHandler(() =>
            {
                Logger.Info("Connection to ClientConsole established ...");

                if (MonitoringStarted) return;
                Monitor(new Dictionary<string, Action<Variant>>
                {
                    {
                        "BoState", strValue =>
                        {
                            CcBoState = strValue.ToString();
                            Logger.Info($"Received value from {Name}.BoState ... '{CcBoState}'.");
                        }
                    },
                    {
                        "IntBoState", intValue =>
                        {
                            CcIntBoState = intValue.ToInt32();
                            Logger.Info($"Received value from {Name}.IntBoState ... {CcIntBoState}.");
                        }
                    },
                    {
                        "FloatBoState", floatValue =>
                        {
                            CcFloatBoState = floatValue.ToFloat();
                            Logger.Info($"Received value from {Name}.FloatBoState ... {CcFloatBoState}.");
                        }
                    }
                });

                MonitoringStarted = true;
            });
        }

        public ClientConsoleClient(string ip, int port) : this(ip, port, "BusinessLogic")
        {
        }

        public ClientConsoleClient() : this("localhost", 48040)
        {
        }

        [UaVariable]
        public string CcBoState { get; set; }

        [UaVariable]
        public int CcIntBoState { get; set; }

        [UaVariable]
        public float CcFloatBoState { get; set; }

        private bool MonitoringStarted { get; set; }
    }
}