using System;
using System.Threading;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;
using UnifiedAutomation.UaSchema;

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
        internal void RaiseSessionIsConnectedEvent(OpcUaSession opcUaSession)
        {
            Logger.Trace("SessionIsConnectedEvent raised!");
            // Raise the event by using the () operator.
            SessionIsConnectedEvent?.Invoke(opcUaSession, new EventArgs());
        }


        private static readonly Lazy<OpcUaSession> Lazy = new Lazy<OpcUaSession>(() =>
        {
            var appSettings = new SecuredApplication();
            var traceSettings = new TraceSettings
            {
                MasterTraceEnabled = true,
                MaxLogFileBackups = 3,
                MaxEntriesPerLog = 10000,
                // TraceFile = @"log\UaclClient.log.txt",
                // DefaultTraceLevel = UnifiedAutomation.UaSchema.TraceLevel.Info
            };

            appSettings.Set<TraceSettings>(traceSettings);
            ApplicationInstance.Default.SetApplicationSettings(appSettings);
            var uaSession = new OpcUaSession(ApplicationInstance.Default);
            return uaSession;
        } , LazyThreadSafetyMode.None);

        /// <summary>
        /// Returns an established Connection to the OPC UA Server
        /// </summary>
        /// <returns></returns>
        public void EstablishOpcUaSession()
        {
            var opcUaSession = this; //new OpcUaSession();
            _uriBuilder = new UriBuilder("opc.tcp", "localhost", 48030);
            do
            {
                try
                {
                    Logger.Info($"Try to connect to:{_uriBuilder.Uri.AbsoluteUri}");
                    opcUaSession.Connect(_uriBuilder.Uri.AbsoluteUri, SecuritySelection.None);
                    Logger.Info($"Connection to {_uriBuilder.Uri.AbsoluteUri} established.");
                    RaiseSessionIsConnectedEvent(opcUaSession);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e,
                        $"An error occurred while try to connect to server:{_uriBuilder.Uri.AbsoluteUri}");
                }
            } while (opcUaSession.ConnectionStatus != ServerConnectionStatus.Connected);
        }

        private static UriBuilder _uriBuilder;

        public static OpcUaSession Instance => Lazy.Value;

        private OpcUaSession(ApplicationInstance application) : base(application)
        {
        }

        /// <summary>
        /// This Method will be invoked, if the ServerConnectionStatus changed from Connected to another Status.
        /// The method will reconnect the Session to the server.
        /// </summary>
        public void Reconnect()
        {
            Logger.Info($"Trying to reconnect to Server URI:{_uriBuilder.Uri.AbsoluteUri}");
            Connect(_uriBuilder.Uri.AbsoluteUri, SecuritySelection.None);
        }
    }
}
