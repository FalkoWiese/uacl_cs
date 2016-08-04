using UnifiedAutomation.UaBase;

namespace UaclClient
{
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
    }
}