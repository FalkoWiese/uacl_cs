using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclClient;
using UaclUtils;

namespace UaclServer
{
    public class LocalProxy
    {
        public LocalProxy() { }

        public LocalProxy(ConnectionInfo info, string path)
        {
            LocalConnection = info;
            Path = path;
        }

        protected void Call<T>(string propertyName, T value)
        {
            GlobalNotfier.FireLcdEvent(Path, propertyName, value);
//            if (LocalConnection == null) return;
//            LocalObject().Write(propertyName, value);
        }

        private RemoteObject LocalObject()
        {
            return _localObject ?? new RemoteObject(LocalConnection.Ip, LocalConnection.Port, $"{LocalConnection.Application}.{Path}");
        }

        private RemoteObject _localObject;

        private ConnectionInfo LocalConnection { get; set; }
        private string Path { get; set; }
    }
}
