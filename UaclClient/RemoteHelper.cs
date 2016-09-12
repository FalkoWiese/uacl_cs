using System;
using System.Collections.Generic;
using System.Linq;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    /// <summary>
    /// Helper Class
    /// - to call Methods,
    /// - to write/read Variables, and
    /// - to monitor Data Changes on a OPC UA Server.
    /// This class should be instanciated within a <see cref="RemoteObject"/> RemoteObject. The associated session is
    /// ment to be instantiated and connected to the desired Server.
    /// </summary>
    public class RemoteHelper
    {
        private readonly OpcUaSession _session;
        private readonly NodeId _parentNode;
        private readonly BrowseContext _browseContext;
        private const int OpcUaIdRootFolder = 84;
        private const int OpcUaIdObjectsFolder = 85;

        private RemoteHelper(OpcUaSession session)
        {
            _session = session;
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

        public RemoteHelper(RemoteObject remoteObject) : this(remoteObject.SessionHandle.Session)
        {
            if (remoteObject.MyNodeId == NodeId.Null)
            {
                remoteObject.MyNodeId = BrowseNodeId(null, remoteObject.Name);
            }
            _parentNode = remoteObject.MyNodeId;
        }

        /// <summary>
        /// Triggers if the connection status of the current session has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverConnectionStatusUpdateEventArgs"></param>
        private void SessionOnConnectionStatusUpdate(Session sender,
            ServerConnectionStatusUpdateEventArgs serverConnectionStatusUpdateEventArgs)
        {
            Logger.Trace($"Connection status has changed to {sender.ConnectionStatus}");
        }

        public NodeId BrowseNodeId(NodeId parentNode, string name, bool recursive = true)
        {
            return PathSeparators().Any(name.Contains)
                ? BrowseNodeIdByPath(parentNode, name)
                : BrowseNodeIdByName(parentNode, name, recursive);
        }

        public NodeId BrowseNodeIdByPath(NodeId parentNode, string path)
        {
            string firstElement;
            var restOfPath = RestOfPath(path, out firstElement);

            var resultNode = BrowseNodeIdByName(parentNode, firstElement, false);
            if (resultNode == null)
            {
                throw new Exception($"Cannot find node for path: '{path}'!");
            }

            return string.IsNullOrEmpty(restOfPath)
                ? resultNode
                : BrowseNodeIdByPath(resultNode, restOfPath);
        }

        public NodeId BrowseNodeIdByName(NodeId parentNode, string nodeName, bool recursive = true)
        {
            NodeId resultNode = null;
            if (parentNode == null)
            {
                parentNode = new NodeId(OpcUaIdRootFolder);
            }

            byte[] continuationPoint;
            var references = _session.Browse(parentNode, _browseContext,
                new RequestSettings {OperationTimeout = 10000}, out continuationPoint);

            foreach (var reference in references)
            {
                var n = new NodeId(reference.NodeId.IdType, reference.NodeId.Identifier,
                    reference.NodeId.NamespaceIndex);

                resultNode = reference.DisplayName.Text == nodeName
                    ? n
                    : null;

                if (resultNode == null && recursive)
                {
                    resultNode = BrowseNodeIdByName(n, nodeName);
                }

                if (resultNode == null) continue;

                Logger.Info($"Found Node ... {reference.DisplayName.Text}");

                break;
            }

            return resultNode;
        }

        public Variant CallMethod(RemoteMethod remoteMethod, ref NodeId methodNodeId)
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

            if (methodNodeId == NodeId.Null)
            {
                methodNodeId = BrowseNodeId(_parentNode, methodName, false);
            }

            List<StatusCode> inputArgumentErrors;
            List<Variant> outputArguments;
            // call the method on the server.
            var result = _session.Call(
                _parentNode,
                methodNodeId,
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

        public static char[] PathSeparators()
        {
            return new[] {'.', ':'};
        }

        public static string RestOfPath(string path, out string firstElement)
        {
            var pathElements = path.Split(PathSeparators(), StringSplitOptions.RemoveEmptyEntries).ToList();

            if (pathElements.Count <= 0)
            {
                throw new Exception($"Cannot find at least one element of given path '{path}'!");
            }

            firstElement = pathElements[0];

            if (pathElements.Count == 1) return "";

            var splitPosition = path.Length - 1;
            foreach (var sep in PathSeparators())
            {
                if (path.Contains(sep))
                {
                    splitPosition = Math.Min(path.IndexOf(sep), splitPosition);
                }
            }

            return path.Substring(splitPosition + 1, path.Length - pathElements[0].Length - 1);
        }

        public void MonitorDataChanges(List<RemoteDataMonitor> monitors, RemoteObject remoteObject)
        {
            var monitoredItems = monitors.Select(m => new DataMonitoredItem(BrowseNodeId(_parentNode, m.Name))
            {
                UserData = m,
                DataChangeTrigger = DataChangeTrigger.StatusValue
            }).Cast<MonitoredItem>().ToList();

            remoteObject.SessionHandle.ClientSubscription().CreateMonitoredItems(monitoredItems,
                new RequestSettings {OperationTimeout = 10000});

            remoteObject.SessionHandle.SetDataChangeHandler(
                (ss, args) =>
                {
                    Logger.Info("Received DATA CHANGE ...");
                    foreach (var dataChange in args.DataChanges)
                    {
                        var remoteDataMonitor = (RemoteDataMonitor) dataChange.MonitoredItem.UserData;
                        remoteDataMonitor?.DataChange(dataChange.Value.WrappedValue);
                    }
                });

            remoteObject.SessionHandle.MonitoredItems.AddRange(monitoredItems);
        }

        public void MonitorDataChange(RemoteDataMonitor monitor, RemoteObject remoteObject)
        {
            var monitoredItems = new List<MonitoredItem>
            {
                new DataMonitoredItem(BrowseNodeId(_parentNode, monitor.Name))
                {
                    UserData = monitor,
                    DataChangeTrigger = DataChangeTrigger.StatusValue
                }
            };

            remoteObject.SessionHandle.ClientSubscription().CreateMonitoredItems(monitoredItems,
                new RequestSettings {OperationTimeout = 10000});

            remoteObject.SessionHandle.SetDataChangeHandler(
                (ss, args) =>
                {
                    Logger.Info("Received DATA CHANGE ...");
                    foreach (var dataChange in args.DataChanges)
                    {
                        var remoteDataMonitor = (RemoteDataMonitor) dataChange.MonitoredItem.UserData;
                        remoteDataMonitor?.DataChange(dataChange.Value.WrappedValue);
                    }
                });

            remoteObject.SessionHandle.MonitoredItems.AddRange(monitoredItems);
        }
    }
}