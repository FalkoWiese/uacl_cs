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
    public class MethodCaller : IMethodCaller
    {
        private readonly OpcUaSession _session;
        private readonly NodeId _parentNode;
        private readonly BrowseContext _browseContext;
        private const int OpcUaIdRootFolder = 84;
        private readonly Dictionary<string, NodeId> _nodeIds;
        private AsyncCallback ResultCallback { get; set; }

        private MethodCaller(OpcUaSession session)
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

        public MethodCaller(OpcUaSession session, string parentNodeName, AsyncCallback methodResult=null) : this(session)
        {
            ResultCallback = methodResult;
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

        /// <summary>
        /// Asynchronously calls a method on a server.
        /// The given <param name="parentNodeName">parentNodeName</param> and <param name="methodName">methodName</param> are 
        /// required to create the corresponding NodeIds to these Nodes on server side.
        /// </summary>
        /// <param name="parentNodeName"> The parent NodeId of the desired Method, needed by the SDK to identify the method</param>
        /// <param name="methodName"> </param>
        /// <param name="arguments"> </param>
        public void BeginCallMethod(string parentNodeName, string methodName, List<Variant> arguments)
        {
            // parse the object id.
            var objectId = BrowseNodeIdByName(null, parentNodeName);

            // get the selected method id.
            var methodId = CreateNodeIdByName(objectId, methodName);

            BeginCallMethod(objectId, methodId, arguments);
        }

        public void BeginCallMethod(NodeId objectId, NodeId methodId, List<Variant> arguments)
        {
            try
            {
                Logger.Trace($"Invoke {methodId} with arguments ...");
                for (var i = 0; i < arguments.Count; i++)
                {
                    Logger.Trace($"\ta[{i}] -> {arguments[i].Value}");
                }

                // call the method.               
                _session.BeginCall(
                    objectId,
                    methodId,
                    arguments,
                    new RequestSettings {OperationTimeout = 10000},
                    ResultCallback ?? DefaultResultCallback,
                    new CallObjectsContainer {Session = _session, Node = methodId});
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "An exception occurred while asynchronously calling method.");
                var parentInfo = objectId?.Identifier.ToString() ?? "parent node is null!";
                var methodInfo = methodId?.Identifier.ToString() ?? "method node is null!";
                Logger.Error($"Exception by calling parent:{parentInfo}, method:{methodInfo}");
            }
        }

        public void BeginCallMethod(string methodName, List<Variant> arguments)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                Logger.Error("Method name is empty!");
                return;
            }

            if (_parentNode == null)
            {
                Logger.Error($"Parent node is null for method '{methodName}'!");
                return;
            }

            BeginCallMethod(_parentNode, CreateNodeIdByName(_parentNode, methodName), arguments);
        }

        /// <summary>
        /// Finishes an asynchronous read request.
        /// </summary>
        private void DefaultResultCallback(IAsyncResult result)
        {
            // get the session used to send the request which was passed as the userData in the Begin call.
            var callObj = (CallObjectsContainer) result.AsyncState;
            try
            {
                // get the results.
                List<StatusCode> inputArgumentErrors;
                List<Variant> outputArguments;
                // call the method.
                var error = callObj.Session.EndCall(
                    result,
                    out inputArgumentErrors,
                    out outputArguments);
                // check for error.
                if (StatusCode.IsBad(error))
                {
                    Logger.Error($"Server returned an error while calling method: {error.ToString(true)}");
                    var identifier = callObj.Node != null ? callObj.Node.Identifier : "Method node object is null!";
                    Logger.Error($"Method: {identifier}");
                    return;
                }
                //Log the inputArgumentErrors
                inputArgumentErrors.ForEach(
                    x => Logger.Trace($"inputArgumentErrors from method call are: {x.ToString()}"));
                //Log the OutputArguments
                outputArguments.ForEach(x => Logger.Trace($"outputArguments from method call are: {x}"));
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "An Exception occurred while finishing the asynchronous method call.");
            }
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
                    new RequestSettings() {OperationTimeout = 10000}, out continuationPoint);

                foreach (var reference in references)
                {
                    var n = new NodeId(reference.NodeId.IdType, reference.NodeId.Identifier,
                        reference.NodeId.NamespaceIndex);
                    resultNode = reference.DisplayName.Text == nodeName ? n : BrowseNodeIdByName(n, nodeName);
                    if (resultNode != null)
                    {
                        if (!_nodeIds.ContainsKey(nodeKey)) _nodeIds.Add(nodeKey, resultNode);
                        break;
                    }
                }
            }

            return resultNode;
        }
    }
}
