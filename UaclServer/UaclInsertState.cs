using System;

namespace UaclServer
{
    /**
     * *UA Method Node* annotion.
     * 
     * This annotation you've to use, if you want to extend a server side method 'Argument List' with a 'State Object',
     * dynamically.
     */
    [AttributeUsage(AttributeTargets.Method)]
    public class UaclInsertState : Attribute
    {
        public UaclInsertState()
        {
        }
    }
}
