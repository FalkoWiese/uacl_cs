using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclClient;
using UaclUtils;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
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
