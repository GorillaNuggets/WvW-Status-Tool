using System.Collections.Generic;
using WvW_Status.Model;

namespace WvW_Status.Factory
{
    internal class NextMatchFactory
    {
        public static List<List<(string, string, bool)>> Create(List<List<MatchInfo>> currentMatch)
        {
            var nextMatch = new List<List<(string, string, bool)>>();

            for (var tier = 0; tier <= 8; tier++)
            {
                var preList = new List<(string, string, bool)>();
                for (var rank = 0; rank <= 2; rank++)
                {
                    preList.Add((currentMatch[tier][rank].Name, currentMatch[tier][rank].LinksToolTip,
                        currentMatch[tier][rank].Locked));
                }

                nextMatch.Add(preList);
            }

            for (var tier = 0; tier <= 2; tier++)
            {
                if (currentMatch[tier][1].VictoryPoints == currentMatch[tier][2].VictoryPoints ||
                    currentMatch[(tier + 1)][0].VictoryPoints == currentMatch[(tier + 1)][1].VictoryPoints) continue;

                var temp = (nextMatch[tier][2]);
                nextMatch[tier][2] = (nextMatch[tier + 1][0]);
                nextMatch[tier + 1][0] = (temp);
            }

            for (var tier = 4; tier <= 7; tier++)
            {
                if (currentMatch[tier][1].VictoryPoints == currentMatch[tier][2].VictoryPoints ||
                    currentMatch[(tier + 1)][0].VictoryPoints == currentMatch[(tier + 1)][1].VictoryPoints) continue;

                var temp = (nextMatch[tier][2]);
                nextMatch[tier][2] = (nextMatch[tier + 1][0]);
                nextMatch[tier + 1][0] = (temp);
            }

            return nextMatch;
        }
    }
}