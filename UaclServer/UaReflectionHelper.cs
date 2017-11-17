using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UaclServer
{
    /**
     * Reflection helper class for a specific server side class - BoCapsule.
     */
    public static class UaReflectionHelper
    {
        public static bool ContainsUaNodes(BoCapsule bo, ref ICollection<object> uaNodeItems)
        {
            foreach (var property in bo.BoModel.GetType().GetProperties())
            {
                var uaObjectListAttribute = property.GetCustomAttribute<UaObjectList>();
                if (uaObjectListAttribute == null) continue;

                uaNodeItems = property.GetValue(bo) as ICollection<object>;
                if (uaNodeItems == null) return true;
            }

            return false;
        }
    }
}