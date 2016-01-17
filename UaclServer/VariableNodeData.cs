using System;
using System.Reflection;
using UnifiedAutomation.UaBase;
using UaclUtils;

namespace UaclServer
{
    internal class VariableNodeData
    {
        public object BusinessObject { get; set; }
        public PropertyInfo Property { get; set; }


        public Variant ReadValue()
        {
            var value = BusinessObject != null ? Property?.GetMethod.Invoke(BusinessObject, null) : null;
            return value != null ? new Variant(value, null) : new Variant();
        }

        private string GetPropertyName()
        {
            return Property?.Name ?? "?";
        }

        public bool WriteValue(object value)
        {
            try
            {
                lock (Property)
                {
                    Property.SetMethod.Invoke(BusinessObject, new object[] {value});
                }
                return true;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot write the value of {GetPropertyName()}.");
                return false;
            }
        }
    }
}