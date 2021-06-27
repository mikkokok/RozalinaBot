using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RozalinaBot.Helpers
{
    public static class TimeConverter
    {
        private static readonly CultureInfo _cultureInfo = new CultureInfo("fi-FI");
        public static string GetCurrentTimeAsString()
        {
            return GetCurrentTime().ToShortTimeString();
        }

        public static DateTime GetCurrentTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
        }

        public static string ConverToString(DateTime time)
        {
            return time.ToString(_cultureInfo);
        }
    }
}
