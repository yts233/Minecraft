using System;

namespace Minecraft.Extensions
{
    public static class DateTimeExtension
    {
        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Ticks / 10000000 - 62135596800;
        }

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            return new DateTime((unixTimeStamp + 62135596800) * 10000000, DateTimeKind.Utc).ToLocalTime();
        }
    }
}