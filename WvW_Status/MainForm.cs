using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using WvW_Status.Factory;
using WvW_Status.Model;
using WvW_Status.Properties;
using WvW_Status.Tool;

// Written by Gorilla under the tutelage of Mag (2019)

namespace WvW_Status
{
    public partial class MainForm : Form
    {
        private static List<List<MatchInfo>> _currentMatch;
        private static List<List<(string, string, bool)>> _nextMatch;
        private static bool _currentRegionIsEU = Settings.Default.defaultRegion; // true = EU : false = NA
        private static int _skirmishNA;
        private static int _skirmishEU;
        private static string _endOfSkirmishNA;
        private static string _endOfSkirmishEU;
        private static string _endOfMatchNA;
        private static string _endOfMatchEU;

        private static DateTime _lastRefresh;
        private static Label _lastRefreshLabel;

        private static readonly Panel DataPanel = new Panel
        {
            Dock = DockStyle.Fill
        };

        public MainForm()
        {
            InitializeComponent();
            StartProgram();
            DisplayInformation();
        }

        public void FetchData()
        {
            _lastRefresh = DateTime.Now;

            //
            // Download the information from Guild Wars 2 Official API
            //

            var client = new WebClient();
            var worldsData = client.DownloadString("https://api.guildwars2.com/v2/worlds?ids=all");
            var worldResult = JsonConvert.DeserializeObject<List<WorldsList>>(worldsData);

            const string matchString = "1-1, 1-2, 1-3, 1-4, 2-1, 2-2, 2-3, 2-4, 2-5";
            var matchesData = client.DownloadString($"https://api.guildwars2.com/v2/wvw/matches?ids={matchString}");
            var matchesResult = JsonConvert.DeserializeObject<List<MatchesList>>(matchesData);
            client.Dispose();

            //
            // Convert API responses to usable data
            //

            var worlds = WorldInfoFactory.Create(worldResult);
            _currentMatch = CurrentMatchFactory.Create(matchesResult, worlds);
            _nextMatch = NextMatchFactory.Create(_currentMatch);

            _skirmishNA = matchesResult[0].Skirmishes.Count;
            _skirmishEU = matchesResult[4].Skirmishes.Count;

            _endOfSkirmishNA = EndOfSkirmish.Calculate(matchesResult[0].Start_Time, _skirmishNA);
            _endOfSkirmishEU = EndOfSkirmish.Calculate(matchesResult[4].Start_Time, _skirmishEU);

            _endOfMatchNA = EndOfMatch.Calculate(matchesResult[0].End_Time);
            _endOfMatchEU = EndOfMatch.Calculate(matchesResult[4].End_Time);
        }

        public void StartProgram()
        {
            //
            // Generate region selector controls
            //

            var buttonNA = new RadioButton
            {
                Location = new Point(32, 14),
                Tag = "NA",
                Text = "NA Servers",
                Checked = !_currentRegionIsEU
            };
            var buttonEU = new RadioButton
            {
                Location = new Point(136, 14),
                Tag = "EU",
                Text = "EU Servers",
                Checked = _currentRegionIsEU
            };

            void RegionClicked(object sender, EventArgs e)
            {
                var rb = sender as RadioButton;
                var clickedValue = (string) rb?.Tag;

                // Avoid re-rendering if clicked Region is already displayed
                //
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (clickedValue)
                {
                    case "EU" when _currentRegionIsEU:
                    case "NA" when !_currentRegionIsEU:
                        return;
                }

                _currentRegionIsEU = clickedValue == "EU";
                DataPanel.Controls.Clear();
                DisplayInformation();
            }

            buttonNA.Click += RegionClicked;
            buttonEU.Click += RegionClicked;

            Controls.Add(buttonNA);
            Controls.Add(buttonEU);

            //
            // Refresh button & label
            //

            var buttonRefresh = new Button
            {
                Location = new Point(540, 14),
                Text = "Refresh"
            };

            buttonRefresh.Click += (sender, e) =>
            {
                FetchData();
                DisplayInformation();
            };

            _lastRefreshLabel = new Label
            {
                Location = new Point(340, 14),
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Loading...",
                Font = new Font(Font.FontFamily.Name, 8),
                Width = 200
            };

            var lastRefreshTimer = new Timer {Interval = 1000};
            lastRefreshTimer.Tick += (source, e) =>
            {
                _lastRefreshLabel.Text = $"Last refresh: {TimeAgo.GetTimeSince(_lastRefresh)} ago";
            };
            lastRefreshTimer.Start();

            Controls.Add(buttonRefresh);
            Controls.Add(_lastRefreshLabel);

            //
            // Generate Panel that holds region-specific controls
            //

            Controls.Add(DataPanel);

            //
            // Initial API data fetch
            //

            FetchData();
        }

        public void DisplayInformation()
        {
            //
            // Display the Information
            //

            Size formSize;
            int startCount;
            int finishCount;
            string formText;

            if (_currentRegionIsEU)
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

            Size = formSize;
            Text = formText;

            int y = 52, count = 1;
            var fontColor = Color.FromArgb(255, 170, 170, 170);

            var tooltip = new ToolTip();

            for (var tier = startCount; tier <= finishCount; tier++)
            {
                var matchLabelPanel = new Panel
                {
                    Location = new Point(12, y),
                    Size = new Size(390, 20),
                    Controls =
                    {
                        new Label
                        {
                            Location = new Point(16, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = $"Current Tier {count} Matchup"
                        },
                        new Label
                        {
                            Location = new Point(214, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = "Rank"
                        },
                        new Label
                        {
                            Location = new Point(274, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = "VP"
                        },
                        new Label
                        {
                            Location = new Point(310, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = "War Score"
                        }
                    }
                };
                var nextLabelPanel = new Panel
                {
                    Location = new Point(408, y),
                    Size = new Size(217, 20),
                    Controls =
                    {
                        new Label
                        {
                            Location = new Point(27, 0),
                            Font = new Font("Cambria", 11, FontStyle.Regular),
                            AutoSize = true,
                            ForeColor = fontColor,
                            Text = $"Next Tier {count} Matchup"
                        }
                    }
                };
                var matchPanel = new Panel
                {
                    Location = new Point(12, y + 20),
                    Size = new Size(390, 100),
                    ForeColor = Color.Gainsboro,
                    BackColor = Color.FromArgb(255, 32, 32, 32),
                    Font = new Font("Cambria", 11, FontStyle.Regular)
                };
                var nextPanel = new Panel
                {
                    Location = new Point(408, y + 20),
                    Size = new Size(217, 100),
                    BackColor = Color.FromArgb(255, 32, 32, 32)
                };

                var ry = 12;
                var rank = "";
                var nextColor = new Color();
                var vpColor = new Color[3];

                if (_currentMatch[tier][0].VictoryPoints == _currentMatch[tier][1].VictoryPoints)
                {
                    vpColor[0] = Color.Salmon;
                    vpColor[1] = Color.Salmon;
                }

                if (_currentMatch[tier][1].VictoryPoints == _currentMatch[tier][2].VictoryPoints)
                {
                    vpColor[1] = Color.Salmon;
                    vpColor[2] = Color.Salmon;
                }

                for (var row = 0; row <= 2; row++)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (row)
                    {
                        case 0:
                            rank = "1st";
                            nextColor = Color.DarkGreen;
                            break;
                        case 1:
                            rank = "2nd";
                            nextColor = Color.DarkBlue;
                            break;
                        case 2:
                            rank = "3rd";
                            nextColor = Color.DarkRed;
                            break;
                    }

                    var teamName = new Label
                    {
                        Location = new Point(12, ry),
                        AutoSize = false,
                        Size = new Size(163, 23),
                        ForeColor = Color.Linen,
                        BackColor = _currentMatch[tier][row].Color,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = _currentMatch[tier][row].Name
                    };

                    tooltip.SetToolTip(teamName, _currentMatch[tier][row].LinksToolTip);
                    matchPanel.Controls.Add(teamName);

                    if (_currentMatch[tier][row].Locked)
                    {
                        var teamLock = new PictureBox
                        {
                            Location = new Point(181, ry),
                            Size = new Size(24, 23),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            BackgroundImage = Resources.Lock
                        };
                        tooltip.SetToolTip(teamLock, "Position permanent until reset");
                        matchPanel.Controls.Add(teamLock);
                    }

                    var teamRank = new Label
                    {
                        Location = new Point(218, ry),
                        AutoSize = false,
                        Size = new Size(32, 23),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = rank
                    };

                    matchPanel.Controls.Add(teamRank);

                    var teamVictoryPoints = new Label
                    {
                        Location = new Point(272, ry),
                        AutoSize = false,
                        Size = new Size(32, 23),
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = vpColor[row],
                        Text = _currentMatch[tier][row].VictoryPoints.ToString()
                    };

                    tooltip.SetToolTip(teamVictoryPoints, _currentMatch[tier][row].VictoryPointsToolTip);
                    matchPanel.Controls.Add(teamVictoryPoints);

                    var teamScore = new Label
                    {
                        Location = new Point(326, ry),
                        AutoSize = false,
                        Size = new Size(40, 23),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = _currentMatch[tier][row].Score.ToString()
                    };

                    matchPanel.Controls.Add(teamScore);

                    var nextName = new Label
                    {
                        Location = new Point(12, ry), //14
                        AutoSize = false,
                        Size = new Size(163, 23),
                        ForeColor = Color.Linen,
                        BackColor = nextColor,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Text = _nextMatch[tier][row].Item1
                    };

                    tooltip.SetToolTip(nextName, _nextMatch[tier][row].Item2);
                    nextPanel.Controls.Add(nextName);

                    if (_nextMatch[tier][row].Item3)
                    {
                        var nextLock = new PictureBox
                        {
                            Location = new Point(183, ry),
                            Size = new Size(24, 23),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            BackgroundImage = Resources.Lock
                        };

                        tooltip.SetToolTip(nextLock, "Position secured");
                        nextPanel.Controls.Add(nextLock);
                    }

                    ry += 26;
                }

                y += 140;
                count++;

                DataPanel.Controls.Add(matchLabelPanel);
                DataPanel.Controls.Add(nextLabelPanel);
                DataPanel.Controls.Add(matchPanel);
                DataPanel.Controls.Add(nextPanel);
            }

            var statusPanel = new Panel
            {
                Location = new Point(12, y - 5),
                Size = new Size(613, 28),
                ForeColor = Color.Gainsboro,
                Font = new Font(@"Cambria", 11, FontStyle.Regular),
                Controls =
                {
                    new Label
                    {
                        Location = new Point(27, 5),
                        AutoSize = true,
                        Text = _currentRegionIsEU
                            ? $"Current Skirmish: {_skirmishEU}/84"
                            : $"Current Skirmish: {_skirmishNA}/84"
                    },
                    new Label
                    {
                        Location = new Point(220, 5),
                        AutoSize = true,
                        Text = _currentRegionIsEU
                            ? $"End of Skirmish: {_endOfSkirmishEU}"
                            : $"End of Skirmish: {_endOfSkirmishNA}"
                    },
                    new Label
                    {
                        Location = new Point(414, 5), //414
                        AutoSize = true,
                        Text = _currentRegionIsEU ? $"End of Match: {_endOfMatchEU}" : $"End of Match: {_endOfMatchNA}"
                    }
                }
            };

            DataPanel.Controls.Add(statusPanel);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Location = Settings.Default.defaultLocation;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.defaultRegion = _currentRegionIsEU;
            Settings.Default.defaultLocation = Location;
            Settings.Default.Save();
        }
    }
}