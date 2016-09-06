using UaclServer;

namespace MultiClientConsole
{
    class MultiClientServer : InternalServer
    {
        public MultiClientServer() : this("localhost", 48050, "MultiClientConsole")
        {
        }

        public MultiClientServer(string ip, int port, string applicationName) : base(ip, port, applicationName)
        {
        }
    }
}
