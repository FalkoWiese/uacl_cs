using System;
using System.Collections.Generic;
using System.Text;
using UaclServer;
using UaclUtils;

namespace MultiClientConsole
{
    [UaObject]
    public class MultiClientHost : ServerSideUaProxy
    {
        public MultiClientHost()
        {
            Items = new List<object>();
        }

        [UaMethod]
        public string CollectData()
        {
            var result = new StringBuilder("");
            foreach (var item in Items)
            {
                var rf = new ReflectionHelper(item);

                string itemResult;
                try
                {
                    itemResult = rf.ReadProperty<string>("CcBoState", "ScBoState");
                }
                catch (Exception e)
                {
                    itemResult = e.Message;
                }

                result.AppendLine($"Result from {rf.ItemName} => {itemResult}");
            }

            return result.ToString();
        }

        [UaObjectList]
        public List<object> Items { get; }
    }
}