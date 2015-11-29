using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclServer
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class UaMethod : System.Attribute
    {
        public string Name { get; private set; }

        public UaMethod(string name=null)
        {
            Name = name;
        }
    }
}
