using System;
using System.Collections.Generic;
using UnifiedAutomation.UaBase;

namespace UaclServer
{
    internal sealed class TypeMapping
    {
        private static readonly TypeMapping instance = new TypeMapping();

        private TypeMapping() { }

        private Dictionary<Type, uint> typeMap;
        private Dictionary<Type, uint> TypeMap
        {
            get
            {
                if (typeMap != null) return typeMap;

                typeMap = new Dictionary<Type, uint>
                {
                    [typeof(bool)] = DataTypes.Boolean,
                    [typeof(byte)] = DataTypes.Byte,
                    [typeof(short)] = DataTypes.Int16,
                    [typeof(int)] = DataTypes.Int32,
                    [typeof(long)] = DataTypes.Int64,
                    [typeof(ushort)] = DataTypes.UInt16,
                    [typeof(uint)] = DataTypes.UInt32,
                    [typeof(ulong)] = DataTypes.UInt64,
                    [typeof(float)] = DataTypes.Float,
                    [typeof(double)] = DataTypes.Double,
                    [typeof(string)] = DataTypes.String,
                };

                return typeMap;
            }
        }

        public uint MapType(Type dataType)
        {
            if (!TypeMap.ContainsKey(dataType))
            {
                throw new Exception($"Cannot find type {dataType.Name} in mapping table!");
            }

            return TypeMap[dataType];
        }

        public static TypeMapping Instance => instance;
    }
}