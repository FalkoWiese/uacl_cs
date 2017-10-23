using System;

namespace UaclServer
{
    /**
     * *UA Object Node* annotion.
     * 
     * This annotation you've to use, if you want to have an *UA Object Node* on server side.
     */
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
