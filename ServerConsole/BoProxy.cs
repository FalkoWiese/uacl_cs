using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    [UaObject]
    public class BoProxy
    {
        private Dictionary<int, string> _information; 
        private Dictionary<int, string> Information()
        {
            return _information ?? (_information = new Dictionary<int, string>());
        }

        [InsertUaState]
        [UaMethod]
        public string GetInformation(object state, int id)
        {
            var retVal = !Information().ContainsKey(id) ? string.Empty : Information()[id];
            Logger.Trace($"GetInformation({id}) => '{retVal}'");
            return retVal;
        }

        [UaMethod]
        public void SetInformation(int id, string name)
        {
            Information()[id] = name;
            Logger.Trace($"SetInformation({id}, '{name}') => void");
        }

        [UaMethod]
        public byte[] GetBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
