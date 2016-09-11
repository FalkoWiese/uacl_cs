using System.Collections.Generic;
using UnifiedAutomation.UaBase;

namespace UaclClient
{
    public class RemoteMethod
    {
        public string Name { get; set; }
        public Variant ReturnValue { get; set; }
        public List<Variant> InputArguments { get; set; }

        public Variant Invoke(RemoteObject remoteObject)
        {
            var methodNodeId = NodeId.Null;
            if (remoteObject.NodeIdCache.ContainsKey(Name))
            {
                methodNodeId = remoteObject.NodeIdCache[Name];
            }

            var result = remoteObject.Execute(() =>
            {
                var invoker = new RemoteHelper(remoteObject);
                return invoker.CallMethod(this, ref methodNodeId);
            });

            if (methodNodeId != NodeId.Null && !remoteObject.NodeIdCache.ContainsKey(Name))
            {
                remoteObject.NodeIdCache[Name] = methodNodeId;
            }

            return result;
        }

        public bool HasReturnValue()
        {
            return !ReturnValue.Equals(Variant.Null);
        }
    }
}