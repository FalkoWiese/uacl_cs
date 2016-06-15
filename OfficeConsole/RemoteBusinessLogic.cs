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
            Monitor<string>("BoState", (string v) => { Logger.Info($"Received value from {Name}.BoState ... '{v}'."); });
        }

        public RemoteBusinessLogic() : this("localhost", 48030, "BusinessLogic")
        {
        }

        [UaMethod]
        public bool CalculateJob(string name, int state)
        {
            return Invoke<bool>("CalculateJob", name, state);
        }

        [UaMethod]
        public string ReadState()
        {
            return Read<string>("BoState");
        }

        [UaMethod]
        public void WriteState(string jobState)
        {
            Write("BoState", jobState);
        }
    }
}
