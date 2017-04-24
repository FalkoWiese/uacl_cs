using System.Collections.Generic;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
    public class BoCapsule
    {
        protected bool Equals(BoCapsule other)
        {
            return Equals(BoId, other.BoId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoCapsule) obj);
        }

        public override int GetHashCode()
        {
            return (BoId != null ? BoId.GetHashCode() : 0);
        }

        private sealed class BoIdEqualityComparer : IEqualityComparer<BoCapsule>
        {
            public bool Equals(BoCapsule x, BoCapsule y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.BoId, y.BoId);
            }

            public int GetHashCode(BoCapsule obj)
            {
                return (obj.BoId != null ? obj.BoId.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<BoCapsule> BoIdComparer { get; } = new BoIdEqualityComparer();

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