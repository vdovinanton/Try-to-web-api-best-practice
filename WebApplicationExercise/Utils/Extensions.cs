﻿using System;

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
            date.ToUniversalTime();
            var timeStr = date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            return timeStr;
        }

        public static DateTime ConvertToDateTimeUtc(this string dateString)
        {
            var timezone = TimeZoneInfo.Utc;
            var offset = DateTimeOffset.Parse(dateString);
            var timeOffset = TimeZoneInfo.ConvertTime(offset, timezone);
            
            return timeOffset.DateTime.ToUniversalTime();
        }
    }
}