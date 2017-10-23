using System;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
    /**
     * Encapsulates the important .NET event to determine Local (server) Data Change Events.
     *
     * Further we can find here the static method definition to fire those events.
     */
    public static class GlobalNotifier
    {
        public static event Action<NodeId, string, object> LocalDataChangeEvent;

        public static void FireLdcEvent(NodeId nodeId, string variable, object value)
        {
            LocalDataChangeEvent?.Invoke(nodeId, variable, value);
        }
    }
}