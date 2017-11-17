namespace UaclUtils
{
    /**
     * Saves the Connection Information.
     */
    public class ConnectionInfo
    {
        public ConnectionInfo(string ip, int port, string application)
        {
            Ip = ip;
            Port = port;
            Application = application;
        }

        public string Ip { get; }

        public string Application { get; }

        public int Port { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj.GetType();
            if (GetType() != t) return false;
            var other = (ConnectionInfo) obj;
            return Ip==other.Ip && Port==other.Port && Application==other.Application;
        }

        public override int GetHashCode()
        {
            return Ip.GetHashCode() ^ Port.GetHashCode() ^ Application.GetHashCode();
        }
    }
}
