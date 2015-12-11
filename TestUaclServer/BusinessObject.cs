using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclServer;

namespace TestUaclServer
{
    [UaObject]
    public class BusinessObject
    {
        public bool Compare(int i, int j)
        {
            return i != j;
        }
    }
}
