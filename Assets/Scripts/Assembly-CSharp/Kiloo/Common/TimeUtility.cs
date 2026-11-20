using System;
using System.Globalization;

namespace Kiloo.Common
{
	public static class TimeUtility
	{
		public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static long ToUnixTime(this DateTime time)
		{
			double totalSeconds = (time.ToUniversalTime() - epoch).TotalSeconds;
			return (long)totalSeconds;
		}

		public static DateTime FromUnixTime(long unixtime)
		{
			return epoch.AddSeconds(unixtime);
		}

		public static string ToJSON(this DateTime value)
		{
			return value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff'Z'", DateTimeFormatInfo.InvariantInfo);
		}

		public static string ToSortableNumber(this DateTime value)
		{
			return value.ToUniversalTime().ToString("yyyyMMddHHmmssfffffff", DateTimeFormatInfo.InvariantInfo);
		}

		public static bool TryParseSortableNumber(string str, out DateTime result)
		{
			return DateTime.TryParseExact(str, "yyyyMMddHHmmssfffffff", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
		}

		public static string ToRFC1123(this DateTime value)
		{
			return value.ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo.RFC1123Pattern);
		}

		public static bool TryParseRFC1123(string str, out DateTime result)
		{
			return DateTime.TryParseExact(str, DateTimeFormatInfo.InvariantInfo.RFC1123Pattern, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
		}

		public static long RFC1123StringToUnixTime(string str)
		{
			DateTime result;
			if (str != null && TryParseRFC1123(str, out result))
			{
				return result.ToUnixTime();
			}
			return 0L;
		}

		public static TimeSpan GetCurrentTimeZoneUTCOffset(this DateTime time)
		{
			return TimeZone.CurrentTimeZone.GetUtcOffset(time);
		}
	}
}
