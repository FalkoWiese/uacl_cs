using System.Reflection;

namespace UaclServer
{
    internal class MethodNodeData
    {
        public object BusinessObject { get; set; }
        public MethodInfo Method { get; set; }
    }
}