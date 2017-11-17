using System;
using UnifiedAutomation.UaBase;

namespace UaclClient
{
    /**
     * To announce a callback for a 'Data Change event' on Server Side, an instance of this class is created. This
     * instance saves all necessary information for those events - BUT - on 'Client Side', for sure.
     */
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
