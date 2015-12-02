using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaSchema;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
    internal class InternalNodeManager : BaseNodeManager
    {
        public InternalNodeManager(ServerManager server, params string[] namespaceUris)
            : base(server, namespaceUris)
        {
            CompanyUri = namespaceUris.Length < 1 || namespaceUris[0].Trim().Length <= 0 ?
                "http://www.company.com" : namespaceUris[0];

            ApplicationUri = namespaceUris.Length < 2 || namespaceUris[1].Trim().Length <= 0 ?
                "application" : namespaceUris[1];

            InstanceNamespaceIndex = AddNamespaceUri($"{CompanyUri}/{ApplicationUri}/instances");
            TypeNamespaceIndex = AddNamespaceUri($"{CompanyUri}/{ApplicationUri}/types");
        }

        public string ApplicationUri { get; set; }
        public string CompanyUri { get; set; }
        private ushort InstanceNamespaceIndex { get; set; }
        private ushort TypeNamespaceIndex { get; set; }

        private InternalServerManager getManager()
        {
            return (InternalServerManager)this.Server;
        }

        public override void Startup()
        {
            Console.WriteLine("InternalNodeManager: Startup()");

            CreateUaServerInterface();

            base.Startup();
        }

        private void CreateUaServerInterface()
        {
            // The root folder, named by the specific application, e.g. 'ServerConsole'.
            var applicationRoot = AddNode(new CreateObjectSettings()
            {
                ParentNodeId = ObjectIds.ObjectsFolder,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                RequestedNodeId = new NodeId(ApplicationUri, InstanceNamespaceIndex),
                BrowseName = new QualifiedName(ApplicationUri, InstanceNamespaceIndex),
                TypeDefinitionId = ObjectTypeIds.FolderType
            });

            // Here we add the registered objects. Unless, they aren't correctly annotated.
            foreach (var model in getManager().BusinessModel)
            {
                var uaObject = model.GetType().GetCustomAttribute<UaObject>();
                var uaObjectName = uaObject.Name ?? model.GetType().Name;
                AddNode(new CreateObjectSettings()
                {
                    ParentNodeId = applicationRoot.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    RequestedNodeId = new NodeId(uaObjectName, InstanceNamespaceIndex),
                    BrowseName = new QualifiedName(uaObjectName, InstanceNamespaceIndex),
                    TypeDefinitionId = ObjectTypeIds.BaseObjectType
                }, model);
            }
        }

        private class VariableNodeData
        {
            public object BusinessObject { get; set; }
            public PropertyInfo Property { get; set; }
        }

        private class MethodNodeData
        {
            public object BusinessObject { get; set; }
            public MethodInfo Method { get; set; }
        }

        private ObjectNode AddNode(CreateObjectSettings settings, object businessObject = null)
        {
            var node = CreateObject(Server.DefaultRequestContext, settings);
            Console.WriteLine($"Created node ... {node.NodeId.Identifier}.");

            if (businessObject == null) return node;

            foreach (var property in businessObject.GetType().GetProperties())
            {
                AddVariable(businessObject, property, node);
            }

            foreach (var method in businessObject.GetType().GetMethods())
            {
                AddMethod(businessObject, method, node);
            }

            return node;
        }

        private void AddMethod(object businessObject, MethodInfo method, ObjectNode node)
        {
            var uaMethodAttribute = method.GetCustomAttribute<UaMethod>();
            if (uaMethodAttribute == null) return;

            var uaMethodName = uaMethodAttribute.Name ?? method.Name;
            var methodNode = CreateMethod(Server.DefaultRequestContext,
                new CreateMethodSettings()
                {
                    ParentNodeId = node.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.HasComponent,
                    RequestedNodeId = new NodeId($"{node.NodeId.Identifier}.{uaMethodName}", InstanceNamespaceIndex),
                    BrowseName = new QualifiedName(uaMethodName, InstanceNamespaceIndex),
                    DisplayName = uaMethodName,
                });

            methodNode.UserData = new MethodNodeData {BusinessObject = businessObject, Method = method};
            Console.WriteLine($"Created method ... {methodNode.NodeId.Identifier}.");
        }

        private void AddVariable(object businessObject, PropertyInfo property, ObjectNode node)
        {
            var uaVariableAttribute = property.GetCustomAttribute<UaVariable>();
            if (uaVariableAttribute == null) return;

            var uaVariableName = uaVariableAttribute.Name ?? property.Name;
            var variableNode = CreateVariable(Server.DefaultRequestContext,
                new CreateVariableSettings()
                {
                    ParentNodeId = node.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.HasComponent,
                    RequestedNodeId = new NodeId($"{node.NodeId.Identifier}.{uaVariableName}", InstanceNamespaceIndex),
                    BrowseName = new QualifiedName(uaVariableName, InstanceNamespaceIndex),
                    TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                    DataType = DataTypeIds.BaseDataType, //TypeMapping.Instance.MapType(property.PropertyType),
                    ValueRank = ValueRanks.Scalar,
                    Value = new Variant(25),
                    AccessLevel = AccessLevels.CurrentReadOrWrite,
                });

            variableNode.UserData = new VariableNodeData {BusinessObject = businessObject, Property = property};
            Console.WriteLine($"Created variable ... {variableNode.NodeId.Identifier}.");
        }

        protected override CallMethodEventHandler GetMethodDispatcher(RequestContext context, MethodHandle methodHandle)
        {
            if (methodHandle.MethodData is MethodNodeData)
            {
                return DispatchControllerMethod;
            }
            return null;
        }

        private StatusCode DispatchControllerMethod(
            RequestContext context,
            MethodHandle methodHandle,
            IList<Variant> inputArguments,
            List<StatusCode> inputArgumentResults,
            List<Variant> outputArguments)
        {
            MethodNodeData data = methodHandle.MethodData as MethodNodeData;

            object returnValue = data?.Method.Invoke(data.BusinessObject, new object[] {});

            return StatusCodes.BadNotImplemented;
        }

        private sealed class TypeMapping
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

        protected override void Read(
            RequestContext context,
            TransactionHandle transaction,
            IList<NodeAttributeOperationHandle> operationHandles,
            IList<ReadValueId> settings)
        {
            foreach (NodeAttributeOperationHandle t in operationHandles)
            {
                // Initialize with bad status
                DataValue dv = new DataValue(new StatusCode(StatusCodes.BadNodeIdUnknown));
                // the data passed to CreateVariable is returned as the UserData in the handle.
                /*
                                SystemAddress address = operationHandles[ii].NodeHandle.UserData as SystemAddress;
                                if (address != null)
                                {
                                    // read the data from the underlying system.
                                    object value = m_system.Read(address.Address, address.Offset);
                                    if (value != null)
                                    {
                                        dv = new DataValue(new Variant(value, null), DateTime.UtcNow);
                                    }
                                }
                */
                // return the data to the caller.
                ((ReadCompleteEventHandler)transaction.Callback)(
                    t,
                    transaction.CallbackData,
                    dv,
                    false);
            }
        }

        protected override void Write(
            RequestContext context,
            TransactionHandle transaction,
            IList<NodeAttributeOperationHandle> operationHandles,
            IList<WriteValue> settings)
        {
            foreach (NodeAttributeOperationHandle t in operationHandles)
            {
                // initialize with bad status
                StatusCode error = StatusCodes.BadNodeIdUnknown;
                // the data passed to CreateVariable is returned as the UserData in the handle.
                /*
                                SystemAddress address = operationHandles[ii].NodeHandle.UserData as SystemAddress;
                                if (address != null)
                                {
                                    error = StatusCodes.Good;
                                    if (!m_system.Write(address.Address, address.Offset, settings[ii].Value.Value))
                                    {
                                        error = StatusCodes.BadUserAccessDenied;
                                    }
                                }
                */
                // return the data to the caller.
                ((WriteCompleteEventHandler)transaction.Callback)(
                    t,
                    transaction.CallbackData,
                    error,
                    false);
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Console.WriteLine(@"InternalNodeManager: Shutdown()");
        }
    }
}