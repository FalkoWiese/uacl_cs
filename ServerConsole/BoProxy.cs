using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclServer;

namespace ServerConsole
{
    [UaObject]
    public class BoProxy
    {
        [InsertUaState]
        [UaMethod]
        public string GetInformation(object state, int id)
        {
            var information = new Dictionary<int, string>
            {
                {0, "Null"},
                {1, "One"},
                {2, "Two"},
                {3, "Three"},
                {4, "Four"},
                {5, "Five"},
            };
            return !information.ContainsKey(id) ? string.Empty : information[id];
        }
    }
}
