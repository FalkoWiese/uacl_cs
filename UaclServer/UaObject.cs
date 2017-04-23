using System;

namespace UaclServer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UaObject : Attribute
    {
        public string Name { get; private set; }

        public UaObject(string name = null)
        {
            Name = name;
        }
    }
}
