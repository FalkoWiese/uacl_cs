using UaclClient;
using UaclServer;
using UaclUtils;

namespace MultiClientConsole
{
    [UaObject]
    class ClientConsoleClient : RemoteObject
    {
        private ClientConsoleClient(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
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
