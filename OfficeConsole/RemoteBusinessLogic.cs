using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        { }

        [UaMethod]
        public bool CalculateJob(string name, int state)
        {
            return Invoke<bool>("CalculateJob", name, state);
        }
    }
}
