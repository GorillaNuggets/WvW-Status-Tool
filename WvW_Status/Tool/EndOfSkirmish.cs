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

            return
                (diff.Hours > 0 ? $"{diff.Hours}h " : "") +
                (diff.TotalMinutes > 0 ? $"{Math.Ceiling(diff.TotalMinutes % 60)}m" : "");
        }
    }
}