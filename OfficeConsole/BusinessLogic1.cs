using System.Collections.Generic;
using UaclServer;

namespace OfficeConsole
{
    [UaObject]
    public class BusinessLogic1 : ServerSideUaProxy
    {
        public BusinessLogic1()
        {
            Items = new List<object>();
        }
        
        [UaMethod]
        public string GetName()
        {
            return "BusinessLogic1";
        }
        
        
        [UaObjectList]
        public List<object> Items { get; set; } 
    }

}