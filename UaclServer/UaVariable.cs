using System;

namespace UaclServer
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UaVariable : Attribute
    {
        public string Name { get; private set; }

        public bool Async { get; private set; }

        public UaVariable(string name = null, bool async = false)
        {
            Name = name;
            Async = async;
        }
    }
}
