using System;

namespace WvW_Status.Tool
{
    internal class EndOfMatch
    {
        public static string Calculate(string endOfMatch)
        {
            var endDate = DateTime.ParseExact(
                endOfMatch,
                "yyyy-MM-ddT" + "HH:mm:ssZ",
                System.Globalization.CultureInfo.InvariantCulture
            );

            var diff = endDate - DateTime.Now.AddMinutes(1);

            var result = diff.Days > 0 ? $"{diff.Days}d " : "";
            result += diff.Hours > 0 ? $"{diff.Hours}h " : "";
            result += diff.Minutes > 0 ? $"{Math.Ceiling(diff.TotalMinutes % 60)}m" : "";

            if (result == "")
            {
                result = "< 1 minute";
            }

            return result;
        }
    }
}