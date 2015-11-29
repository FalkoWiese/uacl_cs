using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclServer
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UaObject : System.Attribute
    {
        public string Name { get; private set; }

        public UaObject(string name = null)
        {
            Name = name;
        }
    }
}
