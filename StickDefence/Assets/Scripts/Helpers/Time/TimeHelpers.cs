using System;
using Tools.Extensions;

namespace Helpers.Time
{
    public static class TimeHelpers
    {
        public static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(1);
        public static DateTime TimeStampToDataTime(long timeStamp) => DateTimeOffset.FromUnixTimeSeconds(timeStamp).ToUniversalTime().DateTime;
        public static long DataTimeToTimeStamp(DateTime dateTime) => dateTime.ToUnixTimestamp();
    }
}