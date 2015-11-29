using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclServer
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class UaVariable : System.Attribute
    {
        public string Name { get; private set; }

        public UaVariable(string name = null)
        {
            Name = name;
        }
    }
}
