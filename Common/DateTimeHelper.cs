using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Windows.ApplicationModel.Resources;

namespace GitMe.Common
{
    public abstract class DateTimeHelper
    {
        public static int round(double x)
        {
            return (int) Math.Round(x, MidpointRounding.AwayFromZero);
        }

        public static string GetFormatedTimeSpan(DateTime young, DateTime old)
        {
            var diff = young - old;
            if (diff.TotalMilliseconds < 1501)
                return Constants.FormatASecondAgo;
            else if (diff.TotalSeconds < 60.0)
                return String.Format(Constants.FormatXSecondsAgo, (round(diff.TotalMilliseconds / 1000.0)).ToString());
            else if (diff.TotalSeconds < 91.0)
                return Constants.FormatAMinuteAgo;
            else if (diff.TotalMinutes < 59.0)
                return String.Format(Constants.FormatXMinutesAgo, (round(diff.TotalSeconds / 60.0)).ToString());
            else if (diff.TotalHours < 1.5)
                return Constants.FormatAnHourAgo;
            else if (diff.TotalHours < 24.0)
                return String.Format(Constants.FormatXHoursAgo, (round(diff.TotalMinutes / 60.0)).ToString());
            else if (diff.TotalDays < 1.5)
                return Constants.FormatADayAgo;
            else if (diff.TotalDays < 16.0)
                return String.Format(Constants.FormatXDaysAgo, (round(diff.TotalHours / 24.0)).ToString());
            else if (diff.TotalDays < 46.0)
                return Constants.FormatAMonthAgo;
            else if (diff.TotalDays < 360.0)
                return String.Format(Constants.FormatXMonthsAgo, round(diff.TotalDays / 3.0));
            else if (diff.TotalDays < 541.0)
                return Constants.FormatAYearAgo;
            else
                return String.Format(Constants.FormatXYearsAgo, round((diff.TotalDays / 360)));
        }
        
        public static string GetFormatedTimeSpanEN(DateTime young, DateTime old)
        {
            var diff = young - old;
            if (diff.TotalMilliseconds < 1501)
                return "a second ago";
            else if (diff.TotalSeconds < 60.0)
                return String.Format("{0} seconds ago", (round(diff.TotalMilliseconds / 1000.0)).ToString());
            else if (diff.TotalSeconds < 91.0)
                return "a minute ago";
            else if (diff.TotalMinutes < 59.0)
                return String.Format("{0} minutes ago", (round(diff.TotalSeconds / 60.0)).ToString());
            else if (diff.TotalHours < 1.5)
                return "an hour ago";
            else if (diff.TotalHours < 24.0)
                return String.Format("{0} hours ago", (round(diff.TotalMinutes / 60.0)).ToString());
            else if (diff.TotalDays < 1.5)
                return "yesterday";
            else if (diff.TotalDays < 16.0)
                return String.Format("{0} days ago", (round(diff.TotalHours / 24.0)).ToString());
            else if (diff.TotalDays < 46.0)
                return "a month ago";
            else if (diff.TotalDays < 360.0)
                return String.Format("{0} months ago", round(diff.TotalDays / 3.0));
            else if (diff.TotalDays < 541.0)
                return "a year ago";
            else
                return String.Format("{0} years ago", round((diff.TotalDays / 360)));
        }

        public static DateTime ParsedOrDefaultDateTime(string timestamp)
        {
            DateTime dt;
            if (DateTime.TryParseExact(timestamp,
                                       Constants.DateTimeFormat,
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.AdjustToUniversal,
                                       out dt))
                return dt;
            else
                return DateTime.UtcNow;
        }

        public static string DateTimeToUtcString(DateTime timestamp)
        {
            return (timestamp.ToUniversalTime()).ToString(Constants.DateTimeFormat);
        }

    }
}
