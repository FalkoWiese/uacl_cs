using System;

namespace UaclServer
{
    /**
     * The multiple *UA Object Property* annotion.
     * 
     * This annotation you've to use, if you want to have more than one *UA Object Node* on server side related to
     * another object.
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class UaObjectList : Attribute
    {
    }
}