using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclClient
{
    public class ConnectionInfo
    {
        public ConnectionInfo(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public string Ip { get; }

        public int Port { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj.GetType();
            if (GetType() != t) return false;
            var other = (ConnectionInfo) obj;
            return Ip==other.Ip && Port==other.Port;
        }

        public override int GetHashCode()
        {
            return Ip.GetHashCode() ^ Port.GetHashCode();
        }
    }
}
