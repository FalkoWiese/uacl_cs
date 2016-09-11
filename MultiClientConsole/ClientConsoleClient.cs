using UaclClient;
using UaclServer;
using UaclUtils;
using UnifiedAutomation.UaClient;

namespace MultiClientConsole
{
    [UaObject]
    class ClientConsoleClient : RemoteObject
    {
        private ClientConsoleClient(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
            AnnounceSessionNotConnectedHandler(DisconnectCallback);
        }

        private void DisconnectCallback(Session s, ServerConnectionStatusUpdateEventArgs args)
        {
            if (s.ConnectionStatus != ServerConnectionStatus.Connected)
            {
                // Here, you can execute some useful stuff to handle the connection, maybe you can call s.Disconnect()
                // or something else. Test it, and give me maybe some feedback.
                Logger.Warn(
                    $"The connection {s} is disconnected, you can find the reason maybe at the arguments ... {args}.");
            }
        }

        public ClientConsoleClient() : this("localhost", 48040, "BusinessLogic")
        {
        }

        [UaMethod]
        public void StartMonitoring()
        {
            if (MonitoringStarted) return;

            Connect();
            Monitor("BoState", strValue =>
            {
                CcBoState = strValue.ToString();
                Logger.Info($"Received value from {Name}.BoState ... '{CcBoState}'.");
            });

            Monitor("IntBoState", intValue =>
            {
                CcIntBoState = intValue.ToInt32();
                Logger.Info($"Received value from {Name}.IntBoState ... {CcIntBoState}.");
            });

            Monitor("FloatBoState", floatValue =>
            {
                CcFloatBoState = floatValue.ToFloat();
                Logger.Info($"Received value from {Name}.FloatBoState ... {CcFloatBoState}.");
            });

            MonitoringStarted = true;
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
