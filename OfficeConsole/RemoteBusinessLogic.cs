using System.Collections.Generic;
using System.Runtime.InteropServices;
using UaclClient;
using UaclServer;
using UaclUtils;
using UnifiedAutomation.UaBase;

namespace OfficeConsole
{
    [UaObject]
    public class RemoteBusinessLogic : RemoteObject
    {
        private RemoteBusinessLogic(string ip, int port, string name) : base(ip, port, name)
        {
            Connect();
            Monitor<string>("BoState", (string v) => { Logger.Info($"Received value from {Name}.BoState ... '{v}'."); });
        }

        public RemoteBusinessLogic() : this("localhost", 48030, "BusinessLogic")
        {
        }

        [UaMethod]
        public void FireSimpleEvent()
        {
            Connect();
            Subscribe("FireSimpleEvent",
                (result) =>
                {
                    Logger.Trace($"{result}");
                });

            Invoke("FireSimpleEvent");
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
    }
}
