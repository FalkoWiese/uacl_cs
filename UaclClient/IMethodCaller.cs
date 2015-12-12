using System.Collections.Generic;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public interface IMethodCaller
    {
        /// <summary>
        /// Triggers if the connection status of the current session has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverConnectionStatusUpdateEventArgs"></param>
        void SessionOnConnectionStatusUpdate(Session sender,
            ServerConnectionStatusUpdateEventArgs serverConnectionStatusUpdateEventArgs);

        /// <summary>
        /// Asynchronously calls a method on a server.
        /// The given <param name="parentNodeName">parentNodeName</param> and <param name="methodName">methodName</param> are required to create the corresponding NodeIds to these Modes at the server.
        /// </summary>
        /// <param name="parentNodeName"> The parent NodeId of the desired Method, needed by the SDK to identify the method</param>
        /// <param name="methodName"> </param>
        /// <param name="arguments"> </param>
        void BeginCallMethod(string parentNodeName, string methodName, List<Variant> arguments);

        void BeginCallMethod(NodeId objectId, NodeId methodId, List<Variant> arguments);
        void BeginCallMethod(string methodName, List<Variant> arguments);
    }
}