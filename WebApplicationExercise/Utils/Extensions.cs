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

        public static DateTime ConvertToDateTimeUtc(this string dateString)
        {
            var timezone = TimeZoneInfo.Utc;

            var utc = DateTimeOffset.Parse(dateString);
            var result = TimeZoneInfo.ConvertTime(utc, timezone);

            return result.DateTime;
        }

        public static string ConvertFromUnixToStringUtc(this double dateString)
        {
            var dateTime = dateString.ConvertFromUnixTimestamp();
            return dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }

        public static double ConvertFromStringToUnix(this string dateString)
        {
            var dateTime = dateString.ConvertToDateTimeUtc();
            return dateTime.ConvertToUnixTimestamp();
        }
    }
}