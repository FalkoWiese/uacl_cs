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
    class ClientConsoleClient : ServerSideUaProxy
    {
        private RemoteObject RemoteCccBo { get; set; }
        private ClientConsoleClient(string ip, int port, string name)
        {
            Items = new List<object>();
            RemoteCccBo = new RemoteObject(ip, port, name);
            RemoteCccBo.Connect();
            MonitoringStarted = false;
            RemoteCccBo.SetDisconnectedHandler(
                (session, args) =>
                {
                    Logger.Info($"NotConnectedHandler called for {session} with status {session.ConnectionStatus.ToString()}.");
            
                    if (session.ConnectionStatus != ServerConnectionStatus.Disconnected) return;

                    Logger.Info($"The connection {session} is disconnected.");
                    MonitoringStarted = false;
                    RemoteCccBo.StartConnectionEstablishment();
                });

            RemoteCccBo.SetConnectedHandler(
                (session, args) =>
                {
                    Logger.Info($"PostConnectionHandler called for {session} with status {session.ConnectionStatus.ToString()}.");
            
                    if (session.ConnectionStatus != ServerConnectionStatus.Connected) return;
                    
                    Logger.Info($"Connection {session} to ClientConsole established.");
                    
                    if (MonitoringStarted) return;
                    MonitoringStarted = true;
                    
                    RemoteCccBo.Monitor(new Dictionary<string, Action<Variant>>
                    {
                        {
                            "BoState", strValue =>
                            {
                                CcBoState = strValue.ToString();
                                Logger.Info($"Received value from {RemoteCccBo.Name}.BoState ... '{CcBoState}'.");
                            }
                        },
                        {
                            "IntBoState", intValue =>
                            {
                                CcIntBoState = intValue.ToInt32();
                                Logger.Info($"Received value from {RemoteCccBo.Name}.IntBoState ... {CcIntBoState}.");
                            }
                        },
                        {
                            "FloatBoState", floatValue =>
                            {
                                CcFloatBoState = floatValue.ToFloat();
                                Logger.Info($"Received value from {RemoteCccBo.Name}.FloatBoState ... {CcFloatBoState}.");
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
        
        [UaObjectList]
        public List<object> Items { get; set; }
    }
}