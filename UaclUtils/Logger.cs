using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UaclUtils
{
    public class Logger
    {
        private enum LogType
        {
            Fatal = -1,
            Error = 0,
            Warn= 1,
            Info = 2,
            Trace = 3
        }

        public static void Fatal(string message)
        {
            new Logger().LogImpl(message, LogType.Fatal);
        }

        public static void Error(string message)
        {
            new Logger().LogImpl(message, LogType.Error);
        }

        public static void Error(Exception exc)
        {
            Error($"Message: {exc.Message}\nStack trace: {exc.StackTrace}");
        }

        public static void Warn(string message)
        {
            new Logger().LogImpl(message, LogType.Warn);
        }

        public static void Info(string message)
        {
            new Logger().LogImpl(message, LogType.Info);
        }

        public static void Trace(string message)
        {
            new Logger().LogImpl(message, LogType.Trace);
        }

        private void LogImpl(string message, LogType type)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "<empty message>";
            }

            var typedMessage = $"[{Enum.GetName(typeof (LogType), type)?.ToUpper()}] {message}";

            if (type == LogType.Fatal || type == LogType.Error)
            {
                Console.Error.WriteLine(typedMessage);
            }
            else
            {
                Console.Out.WriteLine(typedMessage);
            }
        }

    }
}
