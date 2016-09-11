using UaclClient;
using UaclServer;
using UaclUtils;

namespace MultiClientConsole
{
    [UaObject]
    class ServerConsoleClient : RemoteObject
    {
        private ServerConsoleClient(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
        }

        public ServerConsoleClient() : this("localhost", 48030, "BusinessLogic")
        {
        }

        [UaMethod]
        public void StartMonitoring()
        {
            if (MonitoringStarted) return;

            Connect();
            Monitor("BoState", v =>
            {
                ScBoState = v.ToString();
                Logger.Info($"Received value from {Name}.BoState ... '{ScBoState}'.");
            });
            MonitoringStarted = true;
        }

        [UaVariable]
        public string ScBoState { get; set; }

        private bool MonitoringStarted { get; set; }
    }
}
