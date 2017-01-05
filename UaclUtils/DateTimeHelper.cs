using System;

namespace UaclUtils
{
    public class DateTimeHelper
    {
        public static long currentTimeMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}