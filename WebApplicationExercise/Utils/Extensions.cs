using System;

namespace WebApplicationExercise.Utils
{
    public static class Extensions
    {
        public static DateTime ConvertFromUnixTimestamp(this double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static string ConvertToStringUtc(this DateTime date)
        {
            // 2018-08-02T13:50:17.315Z
            // 2018-07-30T11:27:37.024Z

            var timeStr = date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            return timeStr;
        }

        public static DateTime ConvertToDateTimeUtc(this string dateString)
        {
            var timezone = TimeZoneInfo.Utc;
            var utc = DateTimeOffset.Parse(dateString);
            var timeOffset = TimeZoneInfo.ConvertTime(utc, timezone);
            var result = timeOffset.DateTime.ToUniversalTime();
            return result;
        }

        //public static string ConvertFromUnixToStringUtc(this double dateString)
        //{
        //    var dateTime = dateString.ConvertFromUnixTimestamp();
        //    return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        //}

        //public static double ConvertFromStringToUnix(this string dateString)
        //{
        //    var dateTime = dateString.ConvertToDateTimeUtc();
        //    return dateTime.ConvertToUnixTimestamp();
        //}
    }
}