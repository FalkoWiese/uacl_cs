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
                // Disconnect();
                Logger.Warn($"The connection {s} is maybe disconnected, the reason is perhaps to find in {args}.");
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
            Monitor<string>("BoState", (string v) =>
            {
                Logger.Info($"Received value from {Name}.BoState ... '{v}'.");
                CcBoState = v;
            });
            MonitoringStarted = true;
        }

        [UaVariable]
        public string CcBoState { get; set; }

        private bool MonitoringStarted { get; set; }


    }
}
