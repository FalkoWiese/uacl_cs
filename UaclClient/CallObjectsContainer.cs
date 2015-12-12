using UnifiedAutomation.UaBase;

namespace UaclClient
{
    public class CallObjectsContainer
    {
        public OpcUaSession Session { get; set; }
        public NodeId Node { get; set; }
    }
}