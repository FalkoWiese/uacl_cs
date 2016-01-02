using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UaclClient
{
    public class SessionFactory
    {
        private SessionFactory() { }
        private static readonly Lazy<SessionFactory> LazyInstance = new Lazy<SessionFactory>(() => new SessionFactory());
        public static SessionFactory Instance => LazyInstance.Value;

        private readonly Lazy<Dictionary<ConnectionInfo, OpcUaSessionHandle>> _lazySessions =
            new Lazy<Dictionary<ConnectionInfo, OpcUaSessionHandle>>(() => new Dictionary<ConnectionInfo, OpcUaSessionHandle>());

        private Dictionary<ConnectionInfo, OpcUaSessionHandle> Sessions => _lazySessions.Value;

        public OpcUaSessionHandle Create(string ip, int port)
        {
            var connectionInfo = new ConnectionInfo(ip, port);
            if (!Sessions.ContainsKey(connectionInfo))
            {
                var session = OpcUaSession.Create(connectionInfo);
                Sessions[connectionInfo] = new OpcUaSessionHandle(session);
            }

            return Sessions[connectionInfo];
        }
    }
}
