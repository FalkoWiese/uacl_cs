using System.Threading;

namespace UaclClient
{
    public class OpcUaSessionHandle
    {
        public OpcUaSessionHandle(OpcUaSession session, Thread handler=null)
        {
            Handler = handler;
            Session = session;
        }

        public Thread Handler { get; set; }
        public OpcUaSession Session { get; set; } 
    }
}