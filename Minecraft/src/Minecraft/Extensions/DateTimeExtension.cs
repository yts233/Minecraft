using System;

namespace Minecraft.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Cast to unix time stamp
        /// </summary>
        /// <param name="dateTime">Time</param>
        /// <returns></returns>
        /// <remarks>∫¡√Îº∂</remarks>
        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            //return dateTime.ToUniversalTime().Ticks / 10000000 - 62135596800;
            return dateTime.ToUniversalTime().Ticks / 10000 - 62135596800000;
        }

        /// <summary>
        /// Cast to <see cref="DateTime"/>
        /// </summary>
        /// <param name="unixTimeStamp">Unix time stamp</param>
        /// <returns></returns>
        /// <remarks>∫¡√Îº∂</remarks>
        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            //return new DateTime((unixTimeStamp + 62135596800) * 10000000, DateTimeKind.Utc).ToLocalTime();
            return new DateTime((unixTimeStamp + 62135596800000) * 10000, DateTimeKind.Utc).ToLocalTime();
        }
    }
}