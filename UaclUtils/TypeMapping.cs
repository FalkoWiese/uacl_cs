using System;
using UnifiedAutomation.UaBase;

namespace UaclUtils
{
    public sealed class TypeMapping
    {
        private TypeMapping()
        {
        }

        public Variant MapType<T>()
        {
            var dataType = typeof (T);

            if (dataType == typeof(bool)) return ToVariant(false);
            if (dataType == typeof(byte)) return ToVariant(byte.MinValue);
            if (dataType == typeof(short)) return ToVariant(short.MinValue);
            if (dataType == typeof(int)) return ToVariant(int.MinValue);
            if (dataType == typeof(long)) return ToVariant(long.MinValue);
            if (dataType == typeof(ushort)) return ToVariant(ushort.MinValue);
            if (dataType == typeof(uint)) return ToVariant(uint.MinValue);
            if (dataType == typeof(ulong)) return ToVariant(ulong.MinValue);
            if (dataType == typeof(float)) return ToVariant(float.MinValue);
            if (dataType == typeof(double)) return ToVariant(double.MinValue);
            if (dataType == typeof(string)) return ToVariant(string.Empty);
            if (dataType == typeof (byte[])) return ToVariant(new byte[0]);

            throw new Exception($"Cannot find type {dataType.Name} in mapping table!");
        }

        public NodeId MapDataTypeId(Type type)
        {
            if (type == typeof(bool)) return DataTypeIds.Boolean;
            if (type == typeof(byte)) return DataTypeIds.Byte;
            if (type == typeof(short)) return DataTypeIds.Int16;
            if (type == typeof(int)) return DataTypeIds.Int32;
            if (type == typeof(long)) return DataTypeIds.Int64;
            if (type == typeof(ushort)) return DataTypeIds.UInt16;
            if (type == typeof(uint)) return DataTypeIds.UInt32;
            if (type == typeof(ulong)) return DataTypeIds.UInt64;
            if (type == typeof(float)) return DataTypeIds.Float;
            if (type == typeof(double)) return DataTypeIds.Double;
            if (type == typeof(string)) return DataTypeIds.String;
            if (type == typeof (byte[])) return DataTypeIds.ByteString;

            throw new Exception($"Cannot find type {type.Name} in mapping table!");
        }

        public object ToObject(Variant item)
        {
            if (item.DataType == BuiltInType.Boolean) return item.ToBoolean();
            if (item.DataType == BuiltInType.Byte) return item.ToByte();
            if (item.DataType == BuiltInType.Int16) return item.ToInt16();
            if (item.DataType == BuiltInType.Int32) return item.ToInt32();
            if (item.DataType == BuiltInType.Int64) return item.ToInt64();
            if (item.DataType == BuiltInType.UInt16) return item.ToUInt16();
            if (item.DataType == BuiltInType.UInt32) return item.ToUInt32();
            if (item.DataType == BuiltInType.UInt64) return item.ToUInt64();
            if (item.DataType == BuiltInType.Float) return item.ToFloat();
            if (item.DataType == BuiltInType.Double) return item.ToDouble();
            if (item.DataType == BuiltInType.String) return item.ToString();
            if (item.DataType == BuiltInType.ByteString) return item.ToByteString();

            throw new Exception($"Cannot find type {item.DataType} in mapping table!");
        }

        public static TypeMapping Instance { get; } = new TypeMapping();

        public Variant ToVariant(object value)
        {
            Type objectType = value.GetType();

            if (objectType == typeof(bool)) return new Variant((bool)value);
            if (objectType == typeof(byte)) return new Variant((byte)value);
            if (objectType == typeof(short)) return new Variant((short)value);
            if (objectType == typeof(int)) return new Variant((int)value);
            if (objectType == typeof(long)) return new Variant((long)value);
            if (objectType == typeof(ushort)) return new Variant((ushort)value);
            if (objectType == typeof(uint)) return new Variant((uint)value);
            if (objectType == typeof(ulong)) return new Variant((ulong)value);
            if (objectType == typeof(float)) return new Variant((float)value);
            if (objectType == typeof(double)) return new Variant((double)value);
            if (objectType == typeof(string)) return new Variant((string)value);
            if (objectType == typeof (byte[])) return new Variant((byte[])value);

            throw new Exception($"Cannot find type {objectType} in mapping table!");
        }

        public Variant ToVariant(object value, Variant item)
        {
            if (item.DataType == BuiltInType.Boolean) return new Variant((bool)value);
            if (item.DataType == BuiltInType.Byte) return new Variant((byte)value);
            if (item.DataType == BuiltInType.Int16) return new Variant((short)value);
            if (item.DataType == BuiltInType.Int32) return new Variant((int)value);
            if (item.DataType == BuiltInType.Int64) return new Variant((long)value);
            if (item.DataType == BuiltInType.UInt16) return new Variant((ushort)value);
            if (item.DataType == BuiltInType.UInt32) return new Variant((uint)value);
            if (item.DataType == BuiltInType.UInt64) return new Variant((ulong)value);
            if (item.DataType == BuiltInType.Float) return new Variant((float)value);
            if (item.DataType == BuiltInType.Double) return new Variant((double)value);
            if (item.DataType == BuiltInType.String) return new Variant((string)value);
            if (item.DataType == BuiltInType.ByteString) return new Variant((byte[])value);

            throw new Exception($"Cannot find type {item.DataType} in mapping table!");
        }
    }
}