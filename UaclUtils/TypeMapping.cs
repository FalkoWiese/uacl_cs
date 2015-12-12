using System;
using System.Collections.Generic;
using UnifiedAutomation.UaBase;

namespace UaclUtils
{
    public sealed class TypeMapping
    {
        private TypeMapping()
        {
        }

        public uint MapType(Type dataType)
        {
            if (dataType == typeof(bool)) return DataTypes.Boolean;
            if (dataType == typeof(byte)) return DataTypes.Byte;
            if (dataType == typeof(short)) return DataTypes.Int16;
            if (dataType == typeof(int)) return DataTypes.Int32;
            if (dataType == typeof(long)) return DataTypes.Int64;
            if (dataType == typeof(ushort)) return DataTypes.UInt16;
            if (dataType == typeof(uint)) return DataTypes.UInt32;
            if (dataType == typeof(ulong)) return DataTypes.UInt64;
            if (dataType == typeof(float)) return DataTypes.Float;
            if (dataType == typeof(double)) return DataTypes.Double;
            if (dataType == typeof(string)) return DataTypes.String;

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

            throw new Exception($"Cannot find type {item.DataType} in mapping table!");
        }

        public static TypeMapping Instance { get; } = new TypeMapping();

        public Variant ToVariant<T>(object value)
        {
            if (typeof(T) == typeof(bool)) return new Variant((bool)value);
            if (typeof(T) == typeof(byte)) return new Variant((byte)value);
            if (typeof(T) == typeof(short)) return new Variant((short)value);
            if (typeof(T) == typeof(int)) return new Variant((int)value);
            if (typeof(T) == typeof(long)) return new Variant((long)value);
            if (typeof(T) == typeof(ushort)) return new Variant((ushort)value);
            if (typeof(T) == typeof(uint)) return new Variant((uint)value);
            if (typeof(T) == typeof(ulong)) return new Variant((ulong)value);
            if (typeof(T) == typeof(float)) return new Variant((float)value);
            if (typeof(T) == typeof(double)) return new Variant((double)value);
            if (typeof(T) == typeof(string)) return new Variant((string)value);

            throw new Exception($"Cannot find type {typeof(T)} in mapping table!");
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

            throw new Exception($"Cannot find type {item.DataType} in mapping table!");
        }
    }
}