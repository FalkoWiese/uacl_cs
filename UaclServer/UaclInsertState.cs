using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UaclInsertState : System.Attribute
    {
        public UaclInsertState()
        {
        }
    }
}
