using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UaclClient
{
    public class SessionHandler
    {
        private SessionHandler() { }
        private static readonly Lazy<SessionHandler> LazyInstance = new Lazy<SessionHandler>(() => new SessionHandler());
        public static SessionHandler Instance => LazyInstance.Value;

        private readonly Lazy<Dictionary<ConnectionInfo, OpcUaSessionHandle>> _lazySessions =
            new Lazy<Dictionary<ConnectionInfo, OpcUaSessionHandle>>(() => new Dictionary<ConnectionInfo, OpcUaSessionHandle>());

        private Dictionary<ConnectionInfo, OpcUaSessionHandle> Sessions => _lazySessions.Value;

        public OpcUaSessionHandle GetSession(RemoteObject remoteObject)
        {
            var connection = remoteObject.Connection;
            if (!Sessions.ContainsKey(connection))
            {
                var session = OpcUaSession.Create(connection);
                Sessions[connection] = new OpcUaSessionHandle(session);
            }

            return Sessions[connection];
        }
    }
}
