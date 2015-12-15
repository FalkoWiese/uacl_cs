using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UaclClient;
using UaclServer;
using UaclUtils;

namespace OfficeConsole
{
    [UaObject]
    public class RemoteBoProxy : RemoteObject
    {
        public RemoteBoProxy() : base("localhost", 48030, "BoProxy")
        {
        }

        [UaMethod]
        public string Information(int id)
        {
            const string remoteMethodName = "GetInformation";
            try
            {
                string returnValue = Invoke<string>(remoteMethodName, id);
                return returnValue;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error while calling {remoteMethodName}";
                ExceptionHandler.Log(e, errorMessage);
                return errorMessage;
            }
        }
    }
}
