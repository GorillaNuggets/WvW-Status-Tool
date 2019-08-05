using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WvW_Status.Model;
using WvW_Status.Tool;

namespace WvW_Status.Factory
{
    internal class CurrentMatchFactory
    {
        public static List<List<MatchInfo>> Create(List<MatchesList> matches, Dictionary<int, WorldInfo> worlds)
        {
            var currentMatch = new List<List<MatchInfo>>();

            string GenerateMatchInfoTip(IEnumerable<int> list)
            {
                return list.Reverse().Aggregate("",
                    (current, id) =>
                        current + (worlds[id].Name.PadRight(25) + "\t" + worlds[id].Population + "\r\n"));
            }

            foreach (var match in matches)
            {
                var (greenLowestVP, greenHighestVP, greenVPTipText) =
                    VictoryPointStats.Calculate(match.Victory_Points.Green, match.Skirmishes.Count);

                var (blueLowestVP, blueHighestVP, blueVPTipText) =
                    VictoryPointStats.Calculate(match.Victory_Points.Blue, match.Skirmishes.Count);

                var (redLowestVP, redHighestVP, redVPTipText) =
                    VictoryPointStats.Calculate(match.Victory_Points.Red, match.Skirmishes.Count);

                int greenPPT = 0;
                int bluePPT = 0;
                int redPPT = 0;

                for (int mapId = 0; mapId <= 3; mapId++)
                {
                    int objCount = match.Maps[mapId].Objectives.Count;
                    for (int obj = 0; obj <= objCount - 1; obj++)
                    {
                        switch (match.Maps[mapId].Objectives[obj].Owner)
                        {
                            case "Green":
                                greenPPT += match.Maps[mapId].Objectives[obj].Points_Tick;
                                break;

                            case "Blue":
                                bluePPT += match.Maps[mapId].Objectives[obj].Points_Tick;
                                break;

                            case "Red":
                                redPPT += match.Maps[mapId].Objectives[obj].Points_Tick;
                                break;
                        }
                    }
                }

                var matchInfo = new List<MatchInfo>
                {
                    new MatchInfo()
                    {
                        Name = worlds[match.Worlds.Green].Name,
                        Color = Color.DarkGreen,
                        LinksToolTip = GenerateMatchInfoTip(match.All_Worlds.Green),
                        VictoryPoints = match.Victory_Points.Green,
                        LowVictoryPoints = greenLowestVP,
                        HighVictoryPoints = greenHighestVP,
                        VictoryPointsToolTip = greenVPTipText,
                        LongVP = match.Victory_Points.Green + (match.Skirmishes[match.Skirmishes.Count - 1].Scores.Green / 100000),
                        Score = match.Skirmishes[match.Skirmishes.Count - 1].Scores.Green,
                        Locked = false,
                        PPT = greenPPT
                    },
                    new MatchInfo()
                    {
                        Name = worlds[match.Worlds.Blue].Name,
                        Color = Color.DarkBlue,
                        LinksToolTip = GenerateMatchInfoTip(match.All_Worlds.Blue),
                        VictoryPoints = match.Victory_Points.Blue,
                        LowVictoryPoints = blueLowestVP,
                        HighVictoryPoints = blueHighestVP,
                        VictoryPointsToolTip = blueVPTipText,
                        LongVP = match.Victory_Points.Blue + (match.Skirmishes[match.Skirmishes.Count - 1].Scores.Blue / 100000),
                        Score = match.Skirmishes[match.Skirmishes.Count - 1].Scores.Blue,
                        Locked = false,
                        PPT = bluePPT
                    },
                    new MatchInfo()
                    {
                        Name = worlds[match.Worlds.Red].Name,
                        Color = Color.DarkRed,
                        LinksToolTip = GenerateMatchInfoTip(match.All_Worlds.Red),
                        VictoryPoints = match.Victory_Points.Red,
                        LowVictoryPoints = redLowestVP,
                        HighVictoryPoints = redHighestVP,
                        VictoryPointsToolTip = redVPTipText,
                        LongVP = match.Victory_Points.Red + (match.Skirmishes[match.Skirmishes.Count - 1].Scores.Red / 100000),
                        Score = match.Skirmishes[match.Skirmishes.Count - 1].Scores.Red,
                        Locked = false,
                        PPT = redPPT
                    }
                };                

                matchInfo = matchInfo.OrderByDescending(m => m.LongVP).ToList();
                currentMatch.Add(matchInfo);
            }

            // -------------------------------------------------------  Determine if teams are locked  --------------------------------------------------------

            for (var tier = 0; tier <= 8; tier++)
            {
                var lvp = new[]
                {
                    currentMatch[tier][0].LowVictoryPoints,
                    currentMatch[tier][1].LowVictoryPoints,
                    currentMatch[tier][2].LowVictoryPoints
                };

                var hvp = new[]
                {
                    currentMatch[tier][0].HighVictoryPoints,
                    currentMatch[tier][1].HighVictoryPoints,
                    currentMatch[tier][2].HighVictoryPoints
                };

                if (lvp[0] > hvp[1])
                {
                    currentMatch[tier][0].Locked = true;
                }

                if (hvp[1] < lvp[0] && lvp[1] > hvp[2])
                {
                    currentMatch[tier][1].Locked = true;
                }

                if (hvp[2] < lvp[1])
                {
                    currentMatch[tier][2].Locked = true;
                }
            }

            return currentMatch;
        }
    }
}