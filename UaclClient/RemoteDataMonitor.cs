using System;
using UnifiedAutomation.UaBase;

namespace UaclClient
{
    public class RemoteDataMonitor
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        public Action<Variant> Callback { get; set; }

        public void DataChange(Variant value)
        {
            Value = value;
            Callback(value);
        }

        public void Announce(RemoteObject remoteObject)
        {
            remoteObject.Execute(() =>
            {

                var remoteHelper = new RemoteHelper(remoteObject);
                remoteHelper.MonitorDataChange(this, remoteObject);
                return Variant.Null;
            });
        }

    }
}
