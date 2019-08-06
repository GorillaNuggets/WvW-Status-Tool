using System;

namespace WvW_Status.Tool
{
    internal class EndOfSkirmish
    {
        public static string Calculate(string beginningOfMatch, int skirmishCount)
        {
            var endOfSkirmish = DateTime
                .ParseExact(beginningOfMatch, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture)
                .AddHours(skirmishCount * 2);

            var diff = endOfSkirmish - DateTime.Now;

            // Sometimes the API is behind on reporting the latest skirmish, resulting in an invalid `skirmishCount` argument
            // and a negative time diff. This is a work around for the problem:
            if (diff.TotalMinutes < 0)
            {
                return Calculate(beginningOfMatch, skirmishCount + 1);
            }

            return
                (diff.Hours > 0 ? $"{diff.Hours}h " : "") +
                (diff.Minutes > 0 ? $"{Math.Ceiling(diff.TotalMinutes % 60)}m" : "");
        }
    }
}