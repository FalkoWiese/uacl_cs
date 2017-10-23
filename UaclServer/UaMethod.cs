using System;

namespace UaclServer
{
    /**
     * *UA Method Node* annotion.
     * 
     * This annotation you've to use, if you want to have an *UA Method Node* on server side.
     */
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
