using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using WvW_Status.Properties;

// Written by Gorilla under the tutelage of Mag (2019)

namespace WvW_Status
{
    public partial class MainForm : Form
    {
        static List<List<MatchInfo>> currentMatch = new List<List<MatchInfo>>();
        static List<List<(string, string, bool)>> nextMatch = new List<List<(string, string, bool)>>();
        static bool region = Settings.Default.defaultRegion; // true = NA : false = EU
        static int skirmishNA;
        static int skirmishEU;
        static string EndOfMatchNA;
        static string EndOfMatchEU;
        public MainForm()
        {
            InitializeComponent();
            StartProgram();
            DisplayInformation();
        }
        public void StartProgram()
        {
            // -------------------------------------------  Download the information from Guild Wars 2 Offical API  -------------------------------------------

            var client = new WebClient();
            var worldsData = client.DownloadString("https://api.guildwars2.com/v2/worlds?ids=all");
            var worldResult = JsonConvert.DeserializeObject<List<WorldsList>>(worldsData);

            var matchString = "1-1, 1-2, 1-3, 1-4, 2-1, 2-2, 2-3, 2-4, 2-5";
            var matchesData = client.DownloadString($"https://api.guildwars2.com/v2/wvw/matches?ids={matchString}");
            var matchesResult = JsonConvert.DeserializeObject<List<MatchesList>>(matchesData);
            client.Dispose();

            // -------------------------------------------------------------  Assign Global data  -------------------------------------------------------------

            skirmishNA = matchesResult[0].Skirmishes.Count;
            skirmishEU = matchesResult[4].Skirmishes.Count;

            EndOfMatchNA = CalculateEndOfMatch(matchesResult[0].End_Time);
            EndOfMatchEU = CalculateEndOfMatch(matchesResult[4].End_Time);

            // ------------------------------------------------------  Sort the information in to Lists  ------------------------------------------------------

            var teamInfo = new Dictionary<int, WorldInfo>();

            foreach (var world in worldResult)
            {
                teamInfo.Add(world.Id, new WorldInfo() { Name = world.Name, Population = world.Population });
            }

            string GenerateMatchInfoTip(IEnumerable<int> List)
            {
                return List.Reverse().Aggregate("", (current, id) => current + (teamInfo[id].Name.PadRight(25) + "\t" + teamInfo[id].Population + "\r\n"));
            }

            foreach (var matches in matchesResult)
            {
                var matchInfo = new List<MatchInfo>
                {
                    new MatchInfo()
                    {
                        Name = teamInfo[matches.Worlds.Green].Name,
                        Color = Color.DarkGreen,
                        LinksToolTip = GenerateMatchInfoTip(matches.All_Worlds.Green),
                        VictoryPoints = matches.Victory_Points.Green,
                        LowVictoryPoints = CalculateVP(matches.Victory_Points.Green,(matches.Skirmishes.Count)).Item1,
                        HighVictoryPoints = CalculateVP(matches.Victory_Points.Green,(matches.Skirmishes.Count)).Item2,
                        VictoryPointsToolTip = CalculateVP(matches.Victory_Points.Green,(matches.Skirmishes.Count)).Item3,
                        LongVP = Double.Parse($"{matches.Victory_Points.Green}.{matches.Skirmishes[(matches.Skirmishes.Count - 1)].Scores.Green}"),
                        Score = matches.Skirmishes[(matches.Skirmishes.Count - 1)].Scores.Green,
                        Locked = false
                    },
                    new MatchInfo()
                    {
                        Name = teamInfo[matches.Worlds.Blue].Name,
                        Color = Color.DarkBlue,
                        LinksToolTip = GenerateMatchInfoTip(matches.All_Worlds.Blue),
                        VictoryPoints = matches.Victory_Points.Blue,
                        LowVictoryPoints = CalculateVP(matches.Victory_Points.Blue,(matches.Skirmishes.Count)).Item1,
                        HighVictoryPoints = CalculateVP(matches.Victory_Points.Blue,(matches.Skirmishes.Count)).Item2,
                        VictoryPointsToolTip = CalculateVP(matches.Victory_Points.Blue,(matches.Skirmishes.Count)).Item3,
                        LongVP = Double.Parse($"{matches.Victory_Points.Blue}.{matches.Skirmishes[(matches.Skirmishes.Count - 1)].Scores.Blue}"),
                        Score = matches.Skirmishes[(matches.Skirmishes.Count - 1)].Scores.Blue,
                        Locked = false
                    },
                    new MatchInfo()
                    {
                        Name = teamInfo[matches.Worlds.Red].Name,
                        Color = Color.DarkRed,
                        LinksToolTip = GenerateMatchInfoTip(matches.All_Worlds.Red),
                        VictoryPoints = matches.Victory_Points.Red,
                        LowVictoryPoints = CalculateVP(matches.Victory_Points.Red,(matches.Skirmishes.Count)).Item1,
                        HighVictoryPoints = CalculateVP(matches.Victory_Points.Red,(matches.Skirmishes.Count)).Item2,
                        VictoryPointsToolTip = CalculateVP(matches.Victory_Points.Red,(matches.Skirmishes.Count)).Item3,
                        LongVP = Double.Parse($"{matches.Victory_Points.Red}.{matches.Skirmishes[(matches.Skirmishes.Count - 1)].Scores.Red}"),
                        Score = matches.Skirmishes[(matches.Skirmishes.Count - 1)].Scores.Red,
                        Locked = false
                    }
                };

                matchInfo = matchInfo.OrderByDescending(m => m.LongVP).ToList();
                currentMatch.Add(matchInfo);
            }

            // -------------------------------------------------------  Determine if teams are locked  --------------------------------------------------------

            for (int tier = 0; tier <= 8; tier++)
            {
                int[] lvp = new int[] {
                    currentMatch[tier][0].LowVictoryPoints,
                    currentMatch[tier][1].LowVictoryPoints,
                    currentMatch[tier][2].LowVictoryPoints};

                int[] hvp = new int[] {
                    currentMatch[tier][0].HighVictoryPoints,
                    currentMatch[tier][1].HighVictoryPoints,
                    currentMatch[tier][2].HighVictoryPoints};

                if (lvp[0] > hvp[1]) { currentMatch[tier][0].Locked = true; }
                if (hvp[1] < lvp[0] && lvp[1] > hvp[2]) { currentMatch[tier][1].Locked = true; }
                if (hvp[2] < lvp[1]) { currentMatch[tier][2].Locked = true; }
            }

            // -----------------------------------------  Determine if there is a tie and sort the Next Matchup List  -----------------------------------------

            for (int tier = 0; tier <= 8; tier++)
            {
                var preList = new List<(string, string, bool)>();
                for (int rank = 0; rank <= 2; rank++)
                {
                    preList.Add((currentMatch[tier][rank].Name, currentMatch[tier][rank].LinksToolTip, currentMatch[tier][rank].Locked));
                }
                nextMatch.Add(preList);
            }
            for (int tier = 0; tier <= 2; tier++)
            {
                if (currentMatch[tier][1].VictoryPoints != currentMatch[tier][2].VictoryPoints && currentMatch[(tier + 1)][0].VictoryPoints != currentMatch[(tier + 1)][1].VictoryPoints)
                {
                    var temp = (nextMatch[tier][2]);
                    nextMatch[tier][2] = (nextMatch[tier + 1][0]);
                    nextMatch[tier + 1][0] = (temp);
                }
            }
            for (int tier = 4; tier <= 7; tier++)
            {
                if (currentMatch[tier][1].VictoryPoints != currentMatch[tier][2].VictoryPoints && currentMatch[(tier + 1)][0].VictoryPoints != currentMatch[(tier + 1)][1].VictoryPoints)
                {
                    var temp = (nextMatch[tier][2]);
                    nextMatch[tier][2] = (nextMatch[tier + 1][0]);
                    nextMatch[tier + 1][0] = (temp);
                }
            }        
        }
        public void DisplayInformation()
        {
            // ----------------------------------------------------------  Display the Information  -----------------------------------------------------------

            var formSize = new Size();
            var startCount = 0;
            var finishCount = 0;
            var formText = "";

            if (region)
            {
                formSize = new Size(653, 830);
                startCount = 4;
                finishCount = 8;
                formText = "Guild Wars 2 - WvW Status (EU)";
            }
            else
            {
                formSize = new Size(653, 690);
                startCount = 0;
                finishCount = 3;
                formText = "Guild Wars 2 - WvW Status (NA)";
            }
            
            this.Size = formSize;
            this.Text = formText;
            this.Icon = Resources.Commander_tag;

            var buttonNA = new RadioButton()
            {
                Location = new Point(32, 14),
                Text = "NA Servers",
                Checked = !region,                
            };
            var buttonEU = new RadioButton()
            {
                Location = new Point(136, 14),
                Text = "EU Servers",
                Checked = region
            };
            void RegionChanged(object sender, EventArgs e)
            {
                region = !region;
                this.Controls.Clear();
                DisplayInformation();
            }

            buttonEU.CheckedChanged += new EventHandler(RegionChanged);

            this.Controls.Add(buttonNA);
            this.Controls.Add(buttonEU);

            int y = 52, count = 1;
            var fontColor = Color.FromArgb(255, 170, 170, 170);

            for (int tier = startCount; tier <= finishCount; tier++)
            {
                var matchLabelPanel = new Panel()
                {
                    Location = new Point(12, y),
                    Size = new Size(390, 20),
                    Controls =
                    {
                        new Label()
                        {
                            Location = new Point(16, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = $"Current Tier {count} Matchup"
                        },
                        new Label()
                        {
                            Location = new Point(214, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = "Rank"
                        },
                        new Label()
                        {
                            Location = new Point(274, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = "VP"
                        },
                        new Label()
                        {
                            Location = new Point(310, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = "War Score"
                        }
                    }
                };
                var nextLabelPanel = new Panel()
                {
                    Location = new Point(408, y),
                    Size = new Size(217, 20),
                    Controls =
                    {
                        new Label()
                        {
                            Location = new Point(28, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = $"Next Tier {count} Matchup"
                        }
                    }
                };
                var matchPanel = new Panel()
                {
                    Location = new Point(12, y + 20),
                    Size = new Size(390, 100),
                    ForeColor = Color.Gainsboro,
                    BackColor = Color.FromArgb(255, 32, 32, 32),
                    Font = new Font("Cambria", 11, FontStyle.Regular),

                };
                var nextPanel = new Panel()
                {
                    Location = new Point(408, y + 20),
                    Size = new Size(217, 100),
                    BackColor = Color.FromArgb(255, 32, 32, 32),
                };

                int ry = 12;
                string rank = "";
                Color nextColor = new Color();
                Color[] vpColor = new Color[3];

                if (currentMatch[tier][0].VictoryPoints == currentMatch[tier][1].VictoryPoints)
                {
                    vpColor[0] = Color.Red;
                    vpColor[1] = Color.Red;
                }
                if (currentMatch[tier][1].VictoryPoints == currentMatch[tier][2].VictoryPoints)
                {
                    vpColor[1] = Color.Red;
                    vpColor[2] = Color.Red;
                }

                for (int row = 0; row <= 2; row++)
                {
                    if (row == 0) { rank = "1st"; nextColor = Color.DarkGreen; };
                    if (row == 1) { rank = "2nd"; nextColor = Color.DarkBlue; };
                    if (row == 2) { rank = "3rd"; nextColor = Color.DarkRed; };

                    var teamName = new Label()
                    {
                        Location = new Point(12, ry),
                        AutoSize = false,
                        Size = new Size(163, 23),
                        ForeColor = Color.Linen,
                        BackColor = currentMatch[tier][row].Color,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = currentMatch[tier][row].Name
                    };

                    var teamToolTip = new ToolTip();
                    teamToolTip.SetToolTip(teamName, currentMatch[tier][row].LinksToolTip);
                    matchPanel.Controls.Add(teamName);

                    if (currentMatch[tier][row].Locked)
                    {
                        var teamLock = new PictureBox()
                        {
                            Location = new Point(181, ry),
                            Size = new Size(24, 23),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            BackgroundImage = Resources.Lock
                        };
                        var teamLockToolTip = new ToolTip();
                        teamLockToolTip.SetToolTip(teamLock, "Position permanent until reset");
                        matchPanel.Controls.Add(teamLock);
                    }

                    var teamRank = new Label()
                    {
                        Location = new Point(218, ry),
                        AutoSize = false,
                        Size = new Size(32, 23),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = rank
                    };

                    matchPanel.Controls.Add(teamRank);

                    var teamVP = new Label()
                    {
                        Location = new Point(272, ry),
                        AutoSize = false,
                        Size = new Size(32, 23),
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = vpColor[row],
                        Text = currentMatch[tier][row].VictoryPoints.ToString(),
                    };

                    var vpTooltip = new ToolTip();
                    vpTooltip.SetToolTip(teamVP, currentMatch[tier][row].VictoryPointsToolTip);
                    matchPanel.Controls.Add(teamVP);

                    var teamScore = new Label()
                    {
                        Location = new Point(326, ry),
                        AutoSize = false,
                        Size = new Size(40, 23),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = currentMatch[tier][row].Score.ToString()
                    };

                    matchPanel.Controls.Add(teamScore);

                    var nextName = new Label()
                    {
                        Location = new Point(14, ry),
                        AutoSize = false,
                        Size = new Size(163, 23),
                        ForeColor = Color.Linen,
                        BackColor = nextColor,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = nextMatch[tier][row].Item1
                    };

                    var nextNameToolTip = new ToolTip();
                    nextNameToolTip.SetToolTip(nextName, nextMatch[tier][row].Item2);
                    nextPanel.Controls.Add(nextName);

                    if (nextMatch[tier][row].Item3)
                    {
                        var nextLock = new PictureBox()
                        {
                            Location = new Point(183, ry),
                            Size = new Size(24, 23),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            BackgroundImage = Resources.Lock
                        };

                        var nextLockToolTip = new ToolTip();
                        nextLockToolTip.SetToolTip(nextLock, "Position secured");
                        nextPanel.Controls.Add(nextLock);
                    }
                    ry += 26;
                }

                y += 140;
                count++;

                this.Controls.Add(matchLabelPanel);
                this.Controls.Add(nextLabelPanel);
                this.Controls.Add(matchPanel);
                this.Controls.Add(nextPanel);
            }

            var statusPanel = new Panel()
            {
                Location = new Point(12, y - 5),
                Size = new Size(613, 28),
                ForeColor = Color.Gainsboro,
                Font = new Font("Cambria", 11, FontStyle.Regular),
                Controls =
                {
                    new Label()
                    {
                        Location = new Point(27, 5),
                        AutoSize = true,
                        Text = region ?
                        $"Current Skirmish:  {skirmishEU}/84":
                        $"Current Skirmish:  {skirmishNA}/84"
                    },
                    new Label()
                    {
                        Location = new Point(219, 5),
                        AutoSize = true,
                        Text = region ?
                        $"Skirmishes Remaining:  {(84 - skirmishEU)}":
                        $"Skirmishes Remaining:  {(84 - skirmishNA)}"
                    },
                    new Label()
                    {
                        Location = new Point(414, 5),
                        AutoSize = true,
                        Text = region ?
                        $"End of Match:  {EndOfMatchEU}":
                        $"End of Match:  {EndOfMatchNA}"
                    },
                }
            };

            this.Controls.Add(statusPanel);
        }
        public string CalculateEndOfMatch(string EndOfMatch)
        {
            DateTime eom = new DateTime();
            eom = DateTime.ParseExact(EndOfMatch, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan diff = eom - DateTime.Now;
            string result = diff.Days > 0
                    ? $"{diff.Days}d {diff.Hours}h {diff.Minutes}m"
                    : diff.Hours > 0
                        ? $"{diff.Hours}h {diff.Minutes}m"
                        : $"{diff.Minutes}m";
            return result;
        }
        public (int, int, string) CalculateVP(int vp, int skirmish)
        {
            int lvp = vp + ((85 - skirmish) * 3);
            int hvp = vp + ((85 - skirmish) * 5);
            string tip = $"Lowest\t {lvp} \r\nHighest\t {hvp}";
            return (lvp, hvp, tip);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Location = Settings.Default.defaultLocation;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.defaultRegion = region;
            Settings.Default.defaultLocation = this.Location;
            Settings.Default.Save();
        }        
    }
}