using System;
using System.Collections.Generic;
using System.Linq;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    /// <summary>
    /// Class to call a Method on a OPC UA Server
    /// This class can be instanciated with an <see cref="OpcUaSession"/> OpcUaSession, this session is ment to be instantiated and connected to the desired Server.
    /// </summary>
    public class RemoteInvoker
    {
        private readonly OpcUaSession _session;
        private readonly NodeId _parentNode;
        private readonly BrowseContext _browseContext;
        private const int OpcUaIdRootFolder = 84;
        private readonly Dictionary<string, NodeId> _nodeIds;

        private RemoteInvoker(OpcUaSession session)
        {
            _session = session;
            _nodeIds = new Dictionary<string, NodeId>();
            _parentNode = null;
            _browseContext = new BrowseContext
            {
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                MaxReferencesToReturn = 0
            };
            _session.ConnectionStatusUpdate += SessionOnConnectionStatusUpdate;
        }

        public RemoteInvoker(OpcUaSession session, string parentNodeName) : this(session)
        {
            _parentNode = BrowseNodeId(null, parentNodeName);
        }

        /// <summary>
        /// Triggers if the connection status of the current session has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverConnectionStatusUpdateEventArgs"></param>
        private void SessionOnConnectionStatusUpdate(Session sender,
            ServerConnectionStatusUpdateEventArgs serverConnectionStatusUpdateEventArgs)
        {
            //Log.InfoFormat("Connections Status has changed to {0}", sender.ConnectionStatus);
            if (sender.ConnectionStatus == ServerConnectionStatus.Disconnected)
            {
                //If the Session was disconnected from Server try to reconnect.
                Logger.Error("The ConnectionStatus of the Session has changed to Disconnected.");
                _session.Reconnect();
            }
            Logger.Trace($"Connection status has changed to {sender.ConnectionStatus}");
        }

        private string GetNodeKey(NodeId parentNode, string nodeName)
        {
            return parentNode.Identifier.ToString() + '/' + nodeName;
        }

        private NodeId CreateNodeIdByName(NodeId parentNode, string nodeName)
        {
            NodeId resultNode;
            var id = GetNodeKey(parentNode, nodeName);
            if (_nodeIds.ContainsKey(id))
            {
                resultNode = _nodeIds[id];
            }
            else
            {
                var node = new NodeId($"{parentNode.Identifier}.{nodeName}", parentNode.NamespaceIndex);
                _nodeIds.Add(id, node);
                resultNode = node;
            }

            return resultNode;
        }

        public NodeId BrowseNodeId(NodeId parentNode, string name)
        {
            return name.Contains('.')
                ? BrowseNodeIdByPath(null, name)
                : BrowseNodeIdByName(null, name);

        }

        public NodeId BrowseNodeIdByPath(NodeId parentNode, string path)
        {
            string firstElement;
            var restOfPath = RestOfPath(path, out firstElement);

            var resultNode = BrowseNodeIdByName(parentNode, firstElement);
            if (resultNode == null)
            {
                throw new Exception($"Cannot find node for path: '{path}'!");
            }

            return string.IsNullOrEmpty(restOfPath) 
                ? resultNode 
                : BrowseNodeIdByPath(resultNode, restOfPath);
        }

        public NodeId BrowseNodeIdByName(NodeId parentNode, string nodeName)
        {
            NodeId resultNode = null;
            if (parentNode == null)
            {
                parentNode = new NodeId(OpcUaIdRootFolder);
            }

            var nodeKey = nodeName;

            if (_nodeIds.ContainsKey(nodeKey))
            {
                resultNode = _nodeIds[nodeKey];
            }
            else
            {
                byte[] continuationPoint;
                var references = _session.Browse(parentNode, _browseContext,
                    new RequestSettings() {OperationTimeout = 10000}, out continuationPoint);

                foreach (var reference in references)
                {
                    var n = new NodeId(reference.NodeId.IdType, reference.NodeId.Identifier,
                        reference.NodeId.NamespaceIndex);

                    resultNode = reference.DisplayName.Text == nodeName
                        ? n
                        : BrowseNodeIdByName(n, nodeName);

                    if (resultNode == null) continue;

                    Logger.Info($"Found Node ... {reference.DisplayName.Text}");

                    if (!_nodeIds.ContainsKey(nodeKey))
                    {
                        _nodeIds.Add(nodeKey, resultNode);
                    }

                    break;
                }
            }

            return resultNode;
        }

        public Variant CallMethod(RemoteMethod remoteMethod)
        {
            var methodName = remoteMethod.Name;

            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new Exception("Method name is empty!");
            }

            if (_parentNode == null)
            {
                throw new Exception($"Parent node is null for method '{methodName}'!");
            }

            List<StatusCode> inputArgumentErrors;
            List<Variant> outputArguments;
            // call the method on the server.
            var result = _session.Call(
                _parentNode,
                CreateNodeIdByName(_parentNode, methodName),
                remoteMethod.InputArguments,
                out inputArgumentErrors,
                out outputArguments);

            if (remoteMethod.HasReturnValue() && outputArguments.Count > 0)
            {
                remoteMethod.ReturnValue = !result.IsGood() ? Variant.Null : outputArguments[0];
            }

            return remoteMethod.ReturnValue;
        }


        public Variant ReadVariable(RemoteVariable remoteVariable)
        {
            if (string.IsNullOrWhiteSpace(remoteVariable.Name))
            {
                throw new Exception("Method name is empty!");
            }

            if (_parentNode == null)
            {
                throw new Exception($"Parent node is null for method '{remoteVariable.Name}'!");
            }

            var readValue = new ReadValueId
            {
                NodeId = BrowseNodeId(_parentNode, remoteVariable.Name),
                AttributeId = Attributes.Value
            };

            var result = _session.Read(new List<ReadValueId> {readValue}, 0, TimestampsToReturn.Both,
                new RequestSettings {OperationTimeout = 10000});

            if (result == null || result.Count < 1)
            {
                throw new Exception($"Cannot read UA Variable {_parentNode.Identifier}.{remoteVariable.Name} on server.");
            }

            return result[0].WrappedValue;
        }

        public Variant WriteVariable(RemoteVariable remoteVariable)
        {
            if (string.IsNullOrWhiteSpace(remoteVariable.Name))
            {
                throw new Exception("Method name is empty!");
            }

            if (_parentNode == null)
            {
                throw new Exception($"Parent node is null for method '{remoteVariable.Name}'!");
            }

            var writeValue = new WriteValue
            {
                NodeId = BrowseNodeId(_parentNode, remoteVariable.Name),
                AttributeId = Attributes.Value,
                Value = new DataValue() {WrappedValue = remoteVariable.Value}
            };

            var result = _session.Write(new List<WriteValue> {writeValue},
                new RequestSettings {OperationTimeout = 10000});

            if (result == null || result.Count < 1 || result[0] != StatusCodes.Good)
            {
                throw new Exception(
                    $"Cannot write UA Variable {_parentNode.Identifier}.{remoteVariable.Name} on server.");
            }

            return remoteVariable.Value;
        }

        public static string RestOfPath(string path, out string firstElement)
        {
            var pathElements = path.Split('.').ToList();

            if (pathElements.Count <= 0)
            {
                throw new Exception($"Cannot find at least one element of given path '{path}'!");
            }

            firstElement = pathElements[0];

            return pathElements.Count == 1
                ? ""
                : path.Substring(path.IndexOf('.') + 1, path.Length - pathElements[0].Length - 1);
        }
    }
}
