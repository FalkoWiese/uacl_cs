using System;

namespace UaclServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UaclInsertState : Attribute
    {
        public UaclInsertState()
        {
        }
    }
}
