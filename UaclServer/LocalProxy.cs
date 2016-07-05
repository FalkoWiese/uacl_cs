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

        public LocalProxy(string path)
        {
            Path = path;
        }

        protected void Call<T>(string propertyName, T value)
        {
            GlobalNotifier.FireLdcEvent(Path, propertyName, value);
        }

        private string Path { get; set; }
    }
}
