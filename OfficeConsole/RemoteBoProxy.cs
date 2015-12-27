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
        public string GetInformation(int id)
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

        [UaMethod]
        public void SetInformation(int id, string information)
        {
            const string remoteMethodName = "SetInformation";
            try
            {
                Invoke(remoteMethodName, id, information);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error while calling {remoteMethodName}";
                ExceptionHandler.Log(e, errorMessage);
            }
        }

        [UaMethod]
        public byte[] GetBytes(string value)
        {
            var bytes = Invoke<byte[]>("GetBytes", value);
            return bytes;
        }
    }
}
