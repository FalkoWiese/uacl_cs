using UaclServer;

namespace OfficeConsole
{
    [UaObject]
    public class BusinessLogic2 : ServerSideUaProxy
    {
        [UaMethod]
        public string GetName()
        {
            return "BusinessLogic2";
        }
    }
}