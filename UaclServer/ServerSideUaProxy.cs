using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclClient;
using UaclUtils;

namespace UaclServer
{
    public class ServerSideUaProxy
    {
        protected ServerSideUaProxy() { }

        protected ServerSideUaProxy(string path)
        {
            Path = path;
        }

        protected void Call<T>(string propertyName, T value)
        {
            GlobalNotifier.FireLdcEvent(Path, propertyName, value);
        }

        public void AddUaNode(ICollection<object> uaObjects, object uaItem)
        {
            uaObjects.Add(uaItem);
        }

        private string Path { get; set; }
    }
}
