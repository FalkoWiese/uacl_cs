using System;

namespace UaclUtils
{
    public class DateTimeHelper
    {
        /**
         * Delivers the number of milliseconds from 1th January of 1970 00:00::00 a clock to the current timestamp.
         */
        public static long currentTimeMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}