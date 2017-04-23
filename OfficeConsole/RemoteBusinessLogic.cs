using UaclClient;
using UaclServer;
using UaclUtils;

namespace OfficeConsole
{
    [UaObject]
    public class RemoteBusinessLogic : RemoteObject
    {
        private RemoteBusinessLogic(string ip, int port, string name) : base(ip, port, name)
        {
            MonitoringStarted = false;
        }

        public RemoteBusinessLogic() : this("localhost", 48030, "BusinessLogic")
        {
        }

        [UaMethod]
        public bool CalculateJob(string name, int state)
        {
            Connect();
            return Invoke<bool>("CalculateJob", name, state);
        }

        [UaMethod]
        public string ReadState()
        {
            Connect();
            return Read<string>("BoState");
        }

        [UaMethod]
        public void WriteState(string jobState)
        {
            Connect();
            Write("BoState", jobState);
        }

        [UaMethod]
        public void MonitorVariables()
        {
            if (MonitoringStarted) return;

            Connect();
            Monitor("BoState", strValue =>
            {
                BoState = strValue.ToString();
                Logger.Info($"Received value from {Name}.BoState ... '{BoState}'.");
            });

            Monitor("IntBoState", intValue =>
            {
                IntBoState = intValue.ToInt32();
                Logger.Info($"Received value from {Name}.IntBoState ... '{IntBoState}'.");
            });

            Monitor("FloatBoState", floatValue =>
            {
                FloatBoState = floatValue.ToFloat();
                Logger.Info($"Received value from {Name}.FloatBoState ... '{FloatBoState}'.");
            });

            MonitoringStarted = true;
        }

        [UaVariable]
        public string BoState { get; set; }

        [UaVariable]
        public int IntBoState { get; set; }

        [UaVariable]
        public float FloatBoState { get; set; }


        private bool MonitoringStarted { get; set; }
    }
}
