using System;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
    public static class GlobalNotifier
    {
        public static event Action<NodeId, string, object> LocalDataChangeEvent;
        public static event Action<NodeId, string, object> LocalNewEvent;

        public static void FireLdcEvent(NodeId nodeId, string variable, object value)
        {
            LocalDataChangeEvent?.Invoke(nodeId, variable, value);
        }

        public static void FireNewEvent(NodeId nodeId, string eventName, object value)
        {
            LocalNewEvent?.Invoke(nodeId, eventName, value);
        }
    }
}
