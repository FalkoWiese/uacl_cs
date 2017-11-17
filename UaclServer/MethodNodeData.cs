using System.Reflection;

namespace UaclServer
{
    /**
     * Enables a relation between an Object and a Method from it.
     */
    internal class MethodNodeData
    {
        public object BusinessObject { get; set; }
        public MethodInfo Method { get; set; }
    }
}