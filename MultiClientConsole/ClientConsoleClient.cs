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
                    Logger.Info($"NotConnectedHandler called for {session} with status {session.ConnectionStatus.ToString()}.");
            
                    if (session.ConnectionStatus != ServerConnectionStatus.Disconnected) return;

                    Logger.Info($"The connection {session} is disconnected.");
                    MonitoringStarted = false;
                    StartConnectionEstablishment();
                });

            AnnouncePostConnectionEstablishedHandler(
                (session, args) =>
                {
                    Logger.Info($"PostConnectionHandler called for {session} with status {session.ConnectionStatus.ToString()}.");
            
                    if (session.ConnectionStatus != ServerConnectionStatus.Connected) return;
                    
                    Logger.Info($"Connection {session} to ClientConsole established.");
                    
                    if (MonitoringStarted) return;
                    MonitoringStarted = true;
                    
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