using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaclUtils
{
    public class Logger
    {
        public enum LogType
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
            if (message == null) return;

            switch (type)
            {
                case LogType.Fatal:
                case LogType.Error:
                    Console.Error.WriteLine(message);
                    break;
                case LogType.Warn:
                case LogType.Info:
                case LogType.Trace:
                    Console.Out.WriteLine(message);
                    break;
                default:
                    throw new Exception("Cannot categorize the logging output, due to an unknown LOGGING TYPE!");
            }
        }

    }
}
