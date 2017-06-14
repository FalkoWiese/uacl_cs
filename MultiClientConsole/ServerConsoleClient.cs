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
            AnnouncePostConnectionEstablishedHandler(ReconnectCallback);
        }

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

            StartMonitoringImpl();
        }
        
        public ServerConsoleClient(string ip, int port) : this(ip, port, "BusinessLogic")
        {
        }

        public ServerConsoleClient() : this("localhost", 48030)
        {
        }

        private void StartMonitoringImpl()
        {
            if (MonitoringStarted) return;
            MonitoringStarted = true;
            
            Monitor("BoState", v =>
            {
                ScBoState = v.ToString();
                Logger.Info($"Received value from {Name}.BoState ... '{ScBoState}'.");
            });
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