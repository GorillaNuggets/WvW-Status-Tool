using System;
using System.Collections.Generic;

namespace WvW_Status.Model
{
    public class Scores
    {
        public double Red { get; set; }
        public double Blue { get; set; }
        public double Green { get; set; }
    }

    public class Worlds
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
    }

    public class AllWorlds
    {
        public List<int> Red { get; set; }
        public List<int> Blue { get; set; }
        public List<int> Green { get; set; }
    }

    public class Deaths
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
    }

    public class Kills
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
    }

    public class VictoryPoints
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
    }

    public class Scores2
    {
        public double Red { get; set; }
        public double Blue { get; set; }
        public double Green { get; set; }
    }

    public class Scores3
    {
        public double Red { get; set; }
        public double Blue { get; set; }
        public double Green { get; set; }
    }

    public class MapScore
    {
        public string Type { get; set; }
        public Scores3 Scores { get; set; }
    }

    public class Skirmish
    {
        public int Id { get; set; }
        public Scores2 Scores { get; set; }
        public List<MapScore> Map_scores { get; set; }
    }

    public class Scores4
    {
        public double Red { get; set; }
        public double Blue { get; set; }
        public double Green { get; set; }
    }

    public class Objective
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Owner { get; set; }
        public DateTime Last_Flipped { get; set; }
        public int Points_Tick { get; set; }
        public int Points_Capture { get; set; }
        public string Claimed_By { get; set; }
        public DateTime? Claimed_At { get; set; }
        public int? Yaks_DeliveRed { get; set; }
        public List<object> Guild_Upgrades { get; set; }
    }

    public class Deaths2
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
    }

    public class Kills2
    {
        public int Red { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
    }

    public class Map
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Scores4 Scores { get; set; }
        public List<object> Bonuses { get; set; }
        public List<Objective> Objectives { get; set; }
        public Deaths2 Deaths { get; set; }
        public Kills2 Kills { get; set; }
    }

    public class MatchesList
    {
        public string Id { get; set; }
        public string Start_Time { get; set; }
        public string End_Time { get; set; }
        public Scores Scores { get; set; }
        public Worlds Worlds { get; set; }
        public AllWorlds All_Worlds { get; set; }
        public Deaths Deaths { get; set; }
        public Kills Kills { get; set; }
        public VictoryPoints Victory_Points { get; set; }
        public List<Skirmish> Skirmishes { get; set; }
        public List<Map> Maps { get; set; }
    }
}