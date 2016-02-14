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

        private readonly Lazy<Dictionary<RemoteObject, OpcUaSessionHandle>> _lazySessions =
            new Lazy<Dictionary<RemoteObject, OpcUaSessionHandle>>(() => new Dictionary<RemoteObject, OpcUaSessionHandle>());

        private Dictionary<RemoteObject, OpcUaSessionHandle> Sessions => _lazySessions.Value;

        public OpcUaSessionHandle GetSession(RemoteObject remoteObject)
        {
            if (!Sessions.ContainsKey(remoteObject))
            {
                var session = OpcUaSession.Create(remoteObject.Connection);
                Sessions[remoteObject] = new OpcUaSessionHandle(session);
            }

            return Sessions[remoteObject];
        }
    }
}
