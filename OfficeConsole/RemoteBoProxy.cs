using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UaclClient;
using UaclServer;
using UaclUtils;

namespace OfficeConsole
{
    [UaObject]
    public class RemoteBoProxy : RemoteObject
    {
        public RemoteBoProxy() : base("localhost", 48030, "BoProxy")
        {
        }

        [UaMethod]
        public string GetInformation(int id)
        {
            Connect();
            return Invoke<string>("GetInformation", id);
        }

        [UaMethod]
        public void SetInformation(int id, string information)
        {
            Connect();
            Invoke("SetInformation", id, information);
        }

        [UaMethod]
        public byte[] GetBytes(string value)
        {
            Connect();
            return Invoke<byte[]>("GetBytes", value);
        }
    }
}
