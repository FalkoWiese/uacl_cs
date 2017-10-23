using System;

namespace UaclServer
{
    /**
     * *UA Property Node* annotion.
     * 
     * This annotation you've to use, if you want to have an *UA Property Node* on server side.
     */
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
