using System;

namespace WvW_Status.Tool
{
    internal class TimeAgo
    {
        public static string GetTimeSince(DateTime date)
        {
            var ts = DateTime.Now.Subtract(date);
            var days = ts.Days;
            var hours = ts.Hours;
            var minutes = ts.Minutes;
            var seconds = ts.Seconds;

            if (days > 0)
                return $"{days} day{(days == 1 ? "" : "s")}";

            if (hours > 0)
                return $"{hours} hour{(hours == 1 ? "" : "s")}";

            if (minutes > 0)
                return $"{minutes} minute{(minutes == 1 ? "" : "s")}";

            if (seconds > 0)
                return $"{seconds} second{(seconds == 1 ? "" : "s")}";

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (seconds < 0)
                return $"in {Math.Abs(seconds)} seconds";

            return "a bit";
        }
    }
}