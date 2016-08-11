using System.Collections.Generic;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
    public class ServerSideUaProxy
    {
        private static string UA_PROXY_NAME = typeof (ServerSideUaProxy).Name;

        protected ServerSideUaProxy()
        {
            UniqueId = new Dictionary<string, NodeId>();
        }

        protected void Call<T>(string propertyName, T value)
        {
            var uniqueId = UniqueId[propertyName];
            GlobalNotifier.FireLdcEvent(uniqueId, propertyName, value);
        }

        public void AddUaNode(ICollection<object> uaObjects, object uaItem)
        {
            uaObjects.Add(uaItem);
        }

        protected void FireEvent<T>(string eventName, T value)
        {
            var proxyId = UniqueId[UA_PROXY_NAME];
            GlobalNotifier.FireNewEvent(proxyId, eventName, value);
        }

        public Dictionary<string,NodeId> UniqueId { get; set; }

    }
}
