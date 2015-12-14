using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
    internal class InternalNodeManager : BaseNodeManager
    {
        public InternalNodeManager(ServerManager server, params string[] namespaceUris)
            : base(server, namespaceUris)
        {
            CompanyUri = namespaceUris.Length < 1 || namespaceUris[0].Trim().Length <= 0
                ? "http://www.company.com"
                : namespaceUris[0];

            ApplicationUri = namespaceUris.Length < 2 || namespaceUris[1].Trim().Length <= 0
                ? "application"
                : namespaceUris[1];

            InstanceNamespaceIndex = AddNamespaceUri($"{CompanyUri}/{ApplicationUri}/instances");
            TypeNamespaceIndex = AddNamespaceUri($"{CompanyUri}/{ApplicationUri}/types");
        }

        private string ApplicationUri { get; set; }
        private string CompanyUri { get; set; }
        private ushort InstanceNamespaceIndex { get; set; }
        private ushort TypeNamespaceIndex { get; set; }

        private InternalServerManager GetManager()
        {
            return (InternalServerManager) Server;
        }

        public override void SessionClosed(Session session)
        {
            base.SessionClosed(session);
            GetManager().OnDisconnect(session);
        }

        public override void SessionOpened(Session session)
        {
            base.SessionOpened(session);
            GetManager().OnConnect(session);
        }

        public override void Startup()
        {
            Logger.Info(@"InternalNodeManager: Startup()");
            CreateUaServerInterface();
            base.Startup();
        }

        private void CreateUaServerInterface()
        {
            // The root folder, named by the specific application, e.g. 'ServerConsole'.
            var applicationTypeRoot = CreateObjectTypeNode(Server.DefaultRequestContext, new CreateObjectTypeSettings()
            {
                ParentNodeId = ObjectTypeIds.BaseObjectType,
                ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                RequestedNodeId = new NodeId(ApplicationUri, TypeNamespaceIndex),
                BrowseName = new QualifiedName(ApplicationUri, TypeNamespaceIndex),
                DisplayName = ApplicationUri
            });
            AddReference(Server.DefaultRequestContext, ObjectTypeIds.BaseObjectType, ReferenceTypeIds.Organizes, false,
                applicationTypeRoot.NodeId, true);

            var applicationRoot = CreateObject(Server.DefaultRequestContext, new CreateObjectSettings()
            {
                ParentNodeId = ObjectIds.ObjectsFolder,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                RequestedNodeId = new NodeId(ApplicationUri, InstanceNamespaceIndex),
                BrowseName = new QualifiedName(ApplicationUri, InstanceNamespaceIndex),
                TypeDefinitionId = ObjectTypeIds.FolderType
            });
            Logger.Info($"Created root node ... {applicationRoot.NodeId.Identifier}.");

            // Here we add the registered objects. Unless, they aren't correctly annotated.
            foreach (var model in GetManager().BusinessModel)
            {
                AddNode(model, applicationRoot, applicationTypeRoot);
            }
        }

        private void AddNode(object businessObject, ObjectNode rootNode, ObjectTypeNode typeRootNode)
        {
            var uaObject = businessObject.GetType().GetCustomAttribute<UaObject>();
            var uaObjectName = uaObject.Name ?? businessObject.GetType().Name;
            var typeNode = CreateObjectTypeNode(Server.DefaultRequestContext, new CreateObjectTypeSettings()
            {
                ParentNodeId = ObjectTypeIds.BaseObjectType,
                ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                RequestedNodeId = new NodeId(uaObjectName, TypeNamespaceIndex),
                BrowseName = new QualifiedName(uaObjectName, TypeNamespaceIndex),
                DisplayName = uaObjectName
            });
            AddReference(Server.DefaultRequestContext, typeRootNode.NodeId, ReferenceTypeIds.Organizes, false,
                typeNode.NodeId, true);

            var node = CreateObject(Server.DefaultRequestContext, new CreateObjectSettings()
            {
                ParentNodeId = rootNode.NodeId,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                RequestedNodeId = new NodeId(uaObjectName, InstanceNamespaceIndex),
                BrowseName = new QualifiedName(uaObjectName, InstanceNamespaceIndex),
                TypeDefinitionId = ObjectTypeIds.BaseObjectType
            });
            Logger.Info($"Created node ... {node.NodeId.Identifier}.");

            foreach (var property in businessObject.GetType().GetProperties())
            {
                AddVariable(businessObject, property, node, typeNode);
            }

            foreach (var method in businessObject.GetType().GetMethods())
            {
                AddMethod(businessObject, method, node);
            }
        }

        private void AddMethod(object businessObject, MethodInfo method, ObjectNode node)
        {
            var uaMethodAttribute = method.GetCustomAttribute<UaMethod>();
            if (uaMethodAttribute == null) return;

            var uaMethodName = uaMethodAttribute.Name ?? method.Name;
            var settings = new CreateMethodSettings()
            {
                ParentNodeId = node.NodeId,
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                RequestedNodeId = new NodeId($"{node.NodeId.Identifier}.{uaMethodName}", InstanceNamespaceIndex),
                BrowseName = new QualifiedName(uaMethodName, InstanceNamespaceIndex),
                DisplayName = uaMethodName,
                Executable = true,
                InputArguments = new List<Argument>(),
                OutputArguments = new List<Argument>()
            };

            foreach (var parameterInfo in method.GetParameters())
            {
                settings.InputArguments.Add(new Argument
                {
                    DataType = TypeMapping.Instance.MapDataTypeId(parameterInfo.ParameterType),
                    Name = parameterInfo.Name,
                    Description = new LocalizedText("en", parameterInfo.Name),
                    ValueRank = ValueRanks.Scalar,
                });
            }

            if (method.ReturnParameter != null)
            {
                settings.OutputArguments.Add(new Argument
                {
                    DataType = TypeMapping.Instance.MapDataTypeId(method.ReturnParameter.ParameterType),
                    Name = "Result",
                    Description = new LocalizedText("en", "Result"),
                    ValueRank = ValueRanks.Scalar
                });
            }

            var methodNode = CreateMethod(Server.DefaultRequestContext, settings);
            methodNode.UserData = new MethodNodeData {BusinessObject = businessObject, Method = method};
            Logger.Info($"Created method ... {methodNode.NodeId.Identifier}.");
        }

        private void AddVariable(object businessObject, PropertyInfo property, ObjectNode parentNode,
            ObjectTypeNode parentTypeNode)
        {
            var uaVariableAttribute = property.GetCustomAttribute<UaVariable>();
            if (uaVariableAttribute == null) return;
            var variableName = uaVariableAttribute.Name ?? property.Name;

            var variableTypeNode = CreateVariableTypeNode(Server.DefaultRequestContext,
                new CreateVariableTypeSettings()
                {
                    ParentNodeId = parentTypeNode.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                    RequestedNodeId = new NodeId($"{parentTypeNode.NodeId.Identifier}.{variableName}", TypeNamespaceIndex),
                    BrowseName = new QualifiedName(variableName, TypeNamespaceIndex),
                    DataType = TypeMapping.Instance.MapDataTypeId(property.PropertyType),
                });
            AddReference(Server.DefaultRequestContext, parentTypeNode.NodeId, ReferenceTypeIds.Organizes, false, variableTypeNode.NodeId, true);

            var variableNode = CreateVariable(Server.DefaultRequestContext,
                new CreateVariableSettings()
                {
                    ParentNodeId = parentNode.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.HasComponent,
                    RequestedNodeId = new NodeId($"{parentNode.NodeId.Identifier}.{variableName}", InstanceNamespaceIndex),
                    BrowseName = new QualifiedName(variableName, InstanceNamespaceIndex),
                    DisplayName = variableName,
                    Description = new LocalizedText("en", variableName),
                    TypeDefinitionId = variableTypeNode.NodeId,
                    Value = new Variant("None"),
                });
            variableNode.UserData = new VariableNodeData { BusinessObject = businessObject, Property = property };

            Logger.Info($"Created variable ... {variableNode.NodeId.Identifier}.");
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
            try
            {
                var data = methodHandle.MethodData as MethodNodeData;

                if (data == null) return StatusCodes.BadMethodInvalid;
                if (outputArguments.Count > 1) return StatusCodes.BadSyntaxError;
                if (data.Method.GetParameters().Length != inputArguments.Count) return StatusCodes.BadNodeAttributesInvalid;

                var parameterList = inputArguments.Select(ia => TypeMapping.Instance.ToObject(ia)).ToArray();
                var returnValue = data.Method.Invoke(data.BusinessObject, parameterList);
                if (outputArguments.Count > 0 && returnValue != null)
                {
                    outputArguments[0] = TypeMapping.Instance.ToVariant(returnValue, outputArguments[0]);
                }

                return StatusCodes.Good;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Error while invoke a remote method ...");
                return StatusCodes.Bad;
            }
        }

        protected override void Read(
            RequestContext context,
            TransactionHandle transaction,
            IList<NodeAttributeOperationHandle> operationHandles,
            IList<ReadValueId> settings)
        {
            foreach (var processVariableHandle in operationHandles)
            {
                // Initialize with bad status
                var dv = new DataValue(new StatusCode(StatusCodes.BadNodeIdUnknown));
                // the data passed to CreateVariable is returned as the UserData in the handle.
                var processVariable = processVariableHandle.NodeHandle.UserData as VariableNodeData;
                if (processVariable != null)
                {
                    // read the data from the underlying system.
                    dv = new DataValue(processVariable.ReadValue(), DateTime.UtcNow);
                }
                // return the data to the caller.
                ((ReadCompleteEventHandler) transaction.Callback)(processVariableHandle, transaction.CallbackData, dv,
                    false);
            }
        }

        protected override void Write(
            RequestContext context,
            TransactionHandle transaction,
            IList<NodeAttributeOperationHandle> operationHandles,
            IList<WriteValue> settings)
        {
            for (var ii = 0; ii < operationHandles.Count; ii++)
            {
                // initialize with bad status
                StatusCode error = StatusCodes.BadNodeIdUnknown;
                // the data passed to CreateVariable is returned as the UserData in the handle.
                var processVariableHandle = operationHandles[ii];
                var processVariable = processVariableHandle.NodeHandle.UserData as VariableNodeData;
                if (processVariable != null)
                {
                    error = processVariable.WriteValue(settings[ii].Value.Value)
                        ? StatusCodes.Good
                        : StatusCodes.BadUserAccessDenied;
                }
                // return the data to the caller.
                ((WriteCompleteEventHandler) transaction.Callback)(processVariableHandle, transaction.CallbackData,
                    error, false);
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Logger.Info(@"InternalNodeManager: Shutdown()");
        }
    }
}