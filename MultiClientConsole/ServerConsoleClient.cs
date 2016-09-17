using UaclClient;
using UaclServer;
using UaclUtils;
using UnifiedAutomation.UaClient;

namespace MultiClientConsole
{
    [UaObject]
    class ServerConsoleClient : RemoteObject
    {
        private ServerConsoleClient(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
            AnnounceSessionNotConnectedHandler(DisconnectCallback);
            AnnouncePostConnectionEstablishedHandler(() =>
            {
                Logger.Info("Connection to ServerConsole established ...");
                StartMonitoringImpl();
            });
        }

        private void DisconnectCallback(Session s, ServerConnectionStatusUpdateEventArgs args)
        {
            if (s.ConnectionStatus != ServerConnectionStatus.Connected)
            {
                // Here, you can execute some useful stuff to handle the connection, maybe you can call s.Disconnect()
                // or something else. Test it, and give me maybe some feedback.
                Logger.Warn(
                    $"The connection {s} is disconnected, you can find the reason maybe at the arguments ... {args}.");
                MonitoringStarted = false;
            }
        }

        public ServerConsoleClient() : this("localhost", 48030, "BusinessLogic")
        {
        }

        private void StartMonitoringImpl()
        {
            if (MonitoringStarted) return;

            Monitor("BoState", v =>
            {
                ScBoState = v.ToString();
                Logger.Info($"Received value from {Name}.BoState ... '{ScBoState}'.");
            });
            MonitoringStarted = true;
        }

        [UaMethod]
        public void StartMonitoring()
        {
            if (!Connected())
            {
                Connect();
            }

            StartMonitoringImpl();
        }

        [UaVariable]
        public string ScBoState { get; set; }

        private bool MonitoringStarted { get; set; }
    }
}
