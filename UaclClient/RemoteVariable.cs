using UnifiedAutomation.UaBase;

namespace UaclClient
{
    /**
     * An instance of this class is the 'Client Side' representation - something like a stub - of a UA Property Node on
     * Server Side.
     */
    public class RemoteVariable
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        public Variant Read(RemoteObject remoteObject)
        {
            return remoteObject.Execute(() =>
            {
                var invoker = new RemoteHelper(remoteObject);
                return invoker.ReadVariable(this);
            });
        }

        public Variant Write(RemoteObject remoteObject)
        {
            return remoteObject.Execute(() =>
            {
                var invoker = new RemoteHelper(remoteObject);
                return invoker.WriteVariable(this);
            });
        }

        public void Monitor(RemoteObject remoteObject)
        {
            remoteObject.Execute(() =>
            {
                var invoker = new RemoteHelper(remoteObject);
                return invoker.WriteVariable(this);
            });
        }
    }
}