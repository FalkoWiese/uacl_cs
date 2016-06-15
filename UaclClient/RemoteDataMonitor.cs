using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteDataMonitor<T>
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        public T ConvertedValue { get; set; }

        public Action<T> Callback { get; set; }

        public void DataChange(Variant value)
        {
            Value = value;
            ConvertedValue = (T) TypeMapping.Instance.ToObject(value);
            Callback(ConvertedValue);
        }

        public void Announce(RemoteObject remoteObject)
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
