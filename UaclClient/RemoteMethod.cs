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
            return remoteObject.Execute(() =>
            {
                var invoker = new RemoteHelper(remoteObject);
                return invoker.CallMethod(this);
            });
        }

        public bool HasReturnValue()
        {
            return !ReturnValue.Equals(Variant.Null);
        }
    }
}