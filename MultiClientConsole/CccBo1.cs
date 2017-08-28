using System.Collections.Generic;
using UaclClient;
using UaclServer;

namespace MultiClientConsole
{
    [UaObject]
    class CccBo1 : ServerSideUaProxy
    {
        private RemoteObject RemoteCccBo1 { get; set; }
        
        private CccBo1(string ip, int port, string name)
        {
            RemoteCccBo1 = new RemoteObject(ip, port, name);
            Items = new List<object>();
        }
        
        private CccBo1(string ip, int port) : this(ip, port, "BusinessLogic.BusinessLogic1")
        {
        }

        public CccBo1() : this("localhost", 48040)
        {
        }

        [UaMethod]
        public string GetName()
        {
            if (!RemoteCccBo1.Connected())
            {
                RemoteCccBo1.Connect();
            }
            return RemoteCccBo1.Invoke<string>("GetName");
        }

         
        [UaObjectList]
        public List<object> Items { get; set; }
    }
}