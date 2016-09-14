using System;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    /// <summary>
    /// Singleton implementation of A OpcUaSession - to make sure that there is only one Session connected to server.
    /// Only an established Connection will be returned.
    /// A Fully Lazy Session, that will use the base Implementation of <see cref="Session"/>
    /// </summary>
    public sealed class OpcUaSession : Session
    {
        public UriBuilder SessionUri { get; }

        internal static OpcUaSession Create(ConnectionInfo connection)
        {
            return new OpcUaSession(ApplicationInstance.Default, connection);
        }

        private OpcUaSession(ApplicationInstance application, ConnectionInfo connection) : base(application)
        {
            Connection = connection;
            SessionUri = new UriBuilder("opc.tcp", Connection.Ip, Connection.Port);
        }

        private ConnectionInfo Connection { get; }

        public bool NotConnected()
        {
            return ConnectionStatus != ServerConnectionStatus.Connected;
        }
    }
}
