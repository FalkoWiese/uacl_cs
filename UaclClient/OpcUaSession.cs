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
        // Delegate for the SessionIsConnectedEvent.
        public delegate void SessionIsConnectedHandler(object sender, EventArgs e);

        // Declare the event 
        public event SessionIsConnectedHandler SessionIsConnectedEvent;

        // Wrap the event in a protected virtual method
        // to enable derived classes to raise the event.
        private void RaiseSessionIsConnectedEvent(OpcUaSession opcUaSession)
        {
            Logger.Trace("SessionIsConnectedEvent raised!");
            SessionIsConnectedEvent?.Invoke(opcUaSession, new EventArgs());
            SessionIsConnectedEvent = null;
        }

        /// <summary>
        /// Returns an established Connection to the OPC UA Server
        /// </summary>
        /// <returns></returns>
        public void EstablishOpcUaSession()
        {
            do
            {
                try
                {
                    if (ConnectionStatus != ServerConnectionStatus.Connected)
                    {
                        Logger.Info($"Try to connect to:{SessionUri.Uri.AbsoluteUri}");
                        Connect(SessionUri.Uri.AbsoluteUri, SecuritySelection.None);
                    }
                    Logger.Info($"Connection to {SessionUri.Uri.AbsoluteUri} established.");
                    RaiseSessionIsConnectedEvent(this);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e,
                        $"An error occurred while try to connect to server: {SessionUri.Uri.AbsoluteUri}.");
                }
            } while (ConnectionStatus != ServerConnectionStatus.Connected);
        }

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

        /// <summary>
        /// This Method will be invoked, if the ServerConnectionStatus changed from Connected to another Status.
        /// The method will reconnect the Session to the server.
        /// </summary>
        public void Reconnect()
        {
            Logger.Info($"Trying to reconnect to Server URI: {SessionUri.Uri.AbsoluteUri}");
            Connect(SessionUri.Uri.AbsoluteUri, SecuritySelection.None);
        }
    }
}
