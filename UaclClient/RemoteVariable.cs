using System;
using System.Collections.Generic;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteVariable
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        public Variant Read(OpcUaSession session, RemoteObject remoteObject)
        {
            return remoteObject.Execute(() =>
            {
                var invoker = new RemoteInvoker(session, remoteObject.Name);
                return invoker.ReadVariable(this);
            }, session);
        }

        public Variant Write(OpcUaSession session, RemoteObject remoteObject)
        {
            return remoteObject.Execute(() =>
            {
                var invoker = new RemoteInvoker(session, remoteObject.Name);
                return invoker.WriteVariable(this);
            }, session);
        }


    }
}