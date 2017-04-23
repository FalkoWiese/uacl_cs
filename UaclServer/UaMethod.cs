using System;

namespace UaclServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UaMethod : Attribute
    {
        public string Name { get; private set; }

        public UaMethod(string name=null)
        {
            Name = name;
        }
    }
}
