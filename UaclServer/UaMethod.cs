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
        private string name;

        public UaMethod(string name=null)
        {
            this.name = name;
        }
    }
}
