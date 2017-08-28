using UaclClient;
using UaclServer;

namespace MultiClientConsole
{
    [UaObject]
    class CccBo2 : ServerSideUaProxy
    {
        private RemoteObject RemoteCccBo2 { get; set; }
        
        private CccBo2(string ip, int port, string name)
        {
            RemoteCccBo2 = new RemoteObject(ip, port, name);
        }
        
        private CccBo2(string ip, int port) : this(ip, port, "BusinessLogic.BusinessLogic1.BusinessLogic2")
        {
        }

        public CccBo2() : this("localhost", 48040)
        {
        }

        [UaMethod]
        public string GetName()
        {
            if (!RemoteCccBo2.Connected())
            {
                RemoteCccBo2.Connect();
            }
            return RemoteCccBo2.Invoke<string>("GetName");
        }

    }
}