

using System;
using System.Collections.Generic;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    /// <summary>
    /// Class to call a Method on a OPC UA Server
    /// This class can be instanciated with an <see cref="OpcUaSession"/> OpcUaSession, this session is ment to be instantiated and connected to the desired Server.
    /// </summary>
    public class VariableInvoker
    {
        private readonly OpcUaSession _session;
        private readonly NodeId _parentNode;
        private readonly BrowseContext _browseContext;
        private const int OpcUaIdRootFolder = 84;
        private readonly Dictionary<string, NodeId> _nodeIds;

        private VariableInvoker(OpcUaSession session)
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

        public VariableInvoker(OpcUaSession session, string parentNodeName)
            : this(session)
        {
            _parentNode = BrowseNodeIdByName(null, parentNodeName);
        }

        /// <summary>
        /// Triggers if the connection status of the current session has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverConnectionStatusUpdateEventArgs"></param>
        public void SessionOnConnectionStatusUpdate(Session sender,
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

        private NodeId BrowseNodeIdByName(NodeId parentNode, string nodeName)
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
                    new RequestSettings {OperationTimeout = 10000}, out continuationPoint);

                foreach (var reference in references)
                {
                    var n = new NodeId(reference.NodeId.IdType, reference.NodeId.Identifier,
                        reference.NodeId.NamespaceIndex);
                    Logger.Trace(
                        $"Browsed node ... Identifier: {reference.NodeId.Identifier}, DisplayName: {reference.DisplayName.Text}, BrowseName: {reference.BrowseName}");
                    resultNode = reference.DisplayName.Text == nodeName ? n : BrowseNodeIdByName(n, nodeName);
                    if (resultNode == null) continue;
                    if (!_nodeIds.ContainsKey(nodeKey)) _nodeIds.Add(nodeKey, resultNode);
                    break;
                }
            }

            return resultNode;
        }

        public Variant Read(RemoteVariable remoteVariable)
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
                NodeId = BrowseNodeIdByName(_parentNode, remoteVariable.Name),
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

        public void Write(RemoteVariable remoteVariable)
        {
            if (string.IsNullOrWhiteSpace(remoteVariable.Name))
            {
                throw new Exception("Method name is empty!");
            }

            if (_parentNode == null)
            {
                throw new Exception($"Parent node is null for method '{remoteVariable.Name}'!");
            }

            var readValue = new WriteValue
            {
                NodeId = BrowseNodeIdByName(_parentNode, remoteVariable.Name),
                AttributeId = Attributes.Value,
                Value = new DataValue() {WrappedValue = remoteVariable.Value}
            };

            var result = _session.Write(new List<WriteValue> {readValue}, new RequestSettings {OperationTimeout = 10000});

            if (result == null || result.Count < 1 || result[0] != StatusCodes.Good)
            {
                throw new Exception($"Cannot write UA Variable {_parentNode.Identifier}.{remoteVariable.Name} on server.");
            }
        }
    }
}
