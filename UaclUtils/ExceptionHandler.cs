using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace UaclUtils
{

    public class ExceptionHandler
    {
        private Exception Error { get; set; }

        private ExceptionHandler(Exception e)
        {
            Error = e;
        }

        public static void LogAndRaise(Exception e, string message = null)
        {
            Log(e, message);
            throw e;
        }

        public static void Log(Exception e, string message=null)
        {
            new ExceptionHandler(e).LogException(message);
        }

        private void LogException(string message)
        {
            Logger.Info(message);
            Logger.Error(Error);
        }
    }


}
