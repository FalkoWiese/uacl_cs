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
        private string name;

        public UaObject(string name = null)
        {
            this.name = name;
        }
    }
}
