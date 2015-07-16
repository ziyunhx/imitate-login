using System;

namespace TNIdea.Common.Helper
{
    public class TimeHelper
    {
        /// <summary>
		/// change Unix timestamp to csharp DateTime
        /// </summary>
		/// <param name="d">double timestamp</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertIntDateTime(double d)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(d);
            return time;
        }

        /// <summary>
		/// change csharp DateTime to Unix timestamp
        /// </summary>
		/// <param name="time">DateTime</param>
        /// <returns>13位时间戳</returns>
        public static long ConvertDateTimeInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }
    }
}
