using System;

namespace UaclUtils
{
    /**
     * Class what encapsulates some stuff about 'Introspection of the .NET Common Language Runtime (CLR'.
     */
    public class ReflectionHelper
    {
        public ReflectionHelper(object item)
        {
            if (item == null) throw new Exception("The argument <item> cannot be null!");
            Item = item;
            ItemType = Item.GetType();
            ItemName = ItemType.FullName;
        }

        public T ReadProperty<T>(params string[] names)
        {
            foreach (var name in names)
            {
                var description = $"{ItemName}::{name}";
                try
                {
                    var propertyInfo = ItemType.GetProperty(name);
                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    if (!propertyInfo.CanRead)
                    {
                        throw new Exception($"Cannot read {description}");
                    }

                    return (T) propertyInfo.GetValue(Item);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e, $"Cannot read {description} without errors!");
                    throw;
                }
            }

            throw new Exception($"Cannot find {names} at {ItemName}.");
        }

        private object Item { get; }
        private Type ItemType { get; }
        public string ItemName { get; }
    }
}