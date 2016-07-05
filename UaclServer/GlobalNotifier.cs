using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclServer
{
    public static class GlobalNotifier
    {
        public static event Action<string, string,object> LocalDataChangeEvent;

        public static void FireLdcEvent(string path, string variable, object value)
        {
            LocalDataChangeEvent?.Invoke(path, variable, value);
        }
    }
}
