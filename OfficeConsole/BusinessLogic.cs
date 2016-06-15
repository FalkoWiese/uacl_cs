using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UaclServer;

namespace OfficeConsole
{
    [UaObject]
    public class BusinessLogic
    {
        [UaMethod]
        public string GetStatistics(int id)
        {
            if(id < 0) throw new Exception("Cannot find statistics with an ID lesser than zero!");
            return @"{ ""statistics"": [{}, {}, {}]}";
        }
    }
}

