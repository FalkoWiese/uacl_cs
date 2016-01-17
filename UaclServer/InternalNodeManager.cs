using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
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

            var parameterList = method.GetParameters().Select(p => p).ToList();
            if (ExistsInsertUaState(method))
            {
                parameterList.Remove(parameterList[0]);
            }

            foreach (var parameterInfo in parameterList)
            {
                settings.InputArguments.Add(new Argument
                {
                    DataType = TypeMapping.Instance.MapDataTypeId(parameterInfo.ParameterType),
                    Name = parameterInfo.Name,
                    Description = new LocalizedText("en", parameterInfo.Name),
                    ValueRank = ValueRanks.Scalar,
                });
            }

            if (method.ReturnParameter != null && method.ReturnParameter.ParameterType != typeof (void))
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
                    RequestedNodeId =
                        new NodeId($"{parentTypeNode.NodeId.Identifier}.{variableName}", TypeNamespaceIndex),
                    BrowseName = new QualifiedName(variableName, TypeNamespaceIndex),
                    DataType = TypeMapping.Instance.MapDataTypeId(property.PropertyType),
                });
            AddReference(Server.DefaultRequestContext, parentTypeNode.NodeId, ReferenceTypeIds.Organizes, false,
                variableTypeNode.NodeId, true);

            var variableNode = CreateVariable(Server.DefaultRequestContext,
                new CreateVariableSettings()
                {
                    ParentNodeId = parentNode.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.HasComponent,
                    RequestedNodeId =
                        new NodeId($"{parentNode.NodeId.Identifier}.{variableName}", InstanceNamespaceIndex),
                    BrowseName = new QualifiedName(variableName, InstanceNamespaceIndex),
                    DisplayName = variableName,
                    Description = new LocalizedText("en", variableName),
                    TypeDefinitionId = VariableTypeIds.BaseVariableType,
                    Value = new Variant("None"),
                });
            variableNode.UserData = new VariableNodeData {BusinessObject = businessObject, Property = property};
            //SetVariableConfiguration(variableNode.NodeId, variableNode.BrowseName, NodeHandleType.ExternalPolled, variableNode.Value);

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
                var argumentCount = inputArguments.Count + (ExistsInsertUaState(data.Method) ? 1 : 0);
                if (data.Method.GetParameters().Length != argumentCount) return StatusCodes.BadNodeAttributesInvalid;

                var parameterList = new List<object>();
                if (ExistsInsertUaState(data.Method))
                {
                    object handler = null;
                    if (GetManager().GetSessionContext().ContainsKey(context.Session))
                    {
                        handler = GetManager().GetSessionContext()[context.Session];
                    }
                    parameterList.Add(handler);
                }
                parameterList.AddRange(inputArguments.Select(o => TypeMapping.Instance.ToObject(o)));

                var parameterArray = new object[parameterList.Count];
                for (var i = 0; i < parameterArray.Length; i++)
                {
                    parameterArray[i] = parameterList[i];
                }

                var returnValue = data.Method.Invoke(data.BusinessObject, parameterArray);
                if (outputArguments.Count > 0 && returnValue != null)
                {
                    if (returnValue.GetType().IsArray)
                    {
                        Variant returnValueDesc = new Variant(new byte[0]);
                        outputArguments[0] = returnValueDesc;
                    }
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

        private static bool ExistsInsertUaState(MethodInfo method)
        {
            var stateAttributes = method.GetCustomAttributes<UaclInsertState>();
            return stateAttributes != null && stateAttributes.Any();
        }

        protected override DataValue Read(
            RequestContext context,
            NodeAttributeHandle nodeHandle,
            string indexRange,
            QualifiedName dataEncoding)
        {
            var processVariable = nodeHandle?.UserData as VariableNode;
            var processVariableData = processVariable?.UserData as VariableNodeData;

            if (processVariableData == null) return new DataValue("");

            var v = new DataValue(processVariableData.ReadValue());

            return v;
        }

        protected override StatusCode? Write(
            RequestContext context,
            NodeAttributeHandle nodeHandle,
            string indexRange,
            DataValue value)
        {
            if (value == null) return StatusCodes.BadNoDataAvailable;
            if (nodeHandle == null) return StatusCodes.BadNodeAttributesInvalid;

            var processVariable = nodeHandle.UserData as VariableNode;
            var processVariableData = processVariable?.UserData as VariableNodeData;
            if (processVariableData == null) return StatusCodes.BadNoDataAvailable;

            return processVariableData.WriteValue(value.Value)
                ? StatusCodes.Good
                : StatusCodes.Bad;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Logger.Info(@"InternalNodeManager: Shutdown()");
        }
    }
}