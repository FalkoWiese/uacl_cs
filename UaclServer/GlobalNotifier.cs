using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
    public static class GlobalNotifier
    {
        public static event Action<NodeId, string, object> LocalDataChangeEvent;

        public static void FireLdcEvent(NodeId nodeId, string variable, object value)
        {
            LocalDataChangeEvent?.Invoke(nodeId, variable, value);
        }
    }
}
