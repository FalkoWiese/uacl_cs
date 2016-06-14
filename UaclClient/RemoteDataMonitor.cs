using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    class RemoteDataMonitor<T>
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        public Action<T> Callback { get; set; }

        public void Subscribe(OpcUaSessionHandle session, RemoteObject remoteObject)
        {
            // @TODO Implement something like a RemoteSubscriber or integrate the subscribe mechanism to the RemoteInvoker.
            // @TODO Create a MonitoredItem store it to - maybe - the RemoteObject instance. Perhaps in a list to have more than one per Session.
            // @TODO Yeah, that's it! But ... tomorrow, or tonight!
        }
    }
}
