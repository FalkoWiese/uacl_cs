using System;
using UaclUtils;
using UnifiedAutomation.UaBase;

namespace UaclClient
{
    public class RemoteDataMonitor<T>
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        private T ConvertedValue { get; set; }

        public Action<T> Callback { private get; set; }

        public void DataChange(Variant value)
        {
            Value = value;
            ConvertedValue = (T) TypeMapping.Instance.ToObject(Value);
            Callback(ConvertedValue);
        }

        public void Monitor(RemoteObject remoteObject)
        {
            remoteObject.Execute(() =>
            {
                var remoteHelper = new RemoteHelper(remoteObject);
                remoteHelper.MonitorDataChange(this, remoteObject);
                return Variant.Null;
            });
        }
    }
}
