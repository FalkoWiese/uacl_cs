using System;
using System.Collections.Generic;
using UaclUtils;
using UnifiedAutomation.UaBase;

namespace UaclClient
{
    public class RemoteEventSubscription
    {
        public string Name { get; set; }

        public List<Variant> Values { get; set; }

        public Action<List<Variant>> Callback { private get; set; }

        public void FireEvent(List<Variant> values)
        {
            Values = values;
            Callback(Values);
        }

        public void Subscribe(RemoteObject remoteObject)
        {
            remoteObject.Execute(() =>
            {
                var remoteHelper = new RemoteHelper(remoteObject);
                remoteHelper.SubscribeEvent(this, remoteObject);
                return Variant.Null;
            });
        }
    }
}
