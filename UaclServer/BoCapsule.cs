using UnifiedAutomation.UaBase;

namespace UaclServer
{
    public class BoCapsule
    {
        public BoCapsule(object model)
        {
            BoModel = model;
            BoId = NodeId.Null;
        }

        public NodeId BoId { get; set; }
        public object BoModel { get; set; }

        public bool IsRegistered()
        {
            return BoId != NodeId.Null;
        }
    }
}