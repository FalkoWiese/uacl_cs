using System.Collections.Generic;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
    /**
     * THE helper class on Server Side. 
     *
     * It levels up UA Monitoring to standard .NET Eventing.
     *
     * One advice: Don't remove the *AddUaNode Method*. It's called via reflection!
     */
    public class ServerSideUaProxy
    {
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

        public Dictionary<string,NodeId> UniqueId { get; set; }

    }
}
