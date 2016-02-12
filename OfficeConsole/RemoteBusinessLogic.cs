using UaclClient;
using UaclServer;

namespace OfficeConsole
{
    [UaObject]
    public class RemoteBusinessLogic : RemoteObject
    {
        private RemoteBusinessLogic(string ip, int port, string name) : base(ip, port, name)
        {
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
            return Read<string>("State");
        }

        [UaMethod]
        public void WriteState(string jobState)
        {
            Write("State", jobState);
        }
    }
}
