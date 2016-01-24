using System;
using System.Collections.Generic;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteMethod
    {
        public string Name { get; set; }
        public Variant ReturnValue { get; set; }
        public List<Variant> InputArguments { get; set; }

        public Variant Invoke(OpcUaSession session, RemoteObject remoteObject)
        {
            return remoteObject.Execute(() =>
            {
                var invoker = new RemoteInvoker(session, remoteObject.Name);
                return invoker.CallMethod(this);
            }, session);
        }

        public bool HasReturnValue()
        {
            return !ReturnValue.Equals(Variant.Null);
        }
    }
}