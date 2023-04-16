using System;
using System.Collections.Generic;
using Battle.Controller.Field;
using Battle.Data;
using Battle.Data.Characters;
using Battle.Data.Perks;
using Battle.Data.Skills;
using Battle.Data.Stats;
using CleverCrow.Fluid.StatsSystem.StatsContainers;
using UnityEngine;

namespace Battle.Controller
{
    [Serializable]
    public class Character : IFieldObject
    {
        [field: SerializeField] public FighterData Data { get; private set; }
        public bool IsHero => Data.IsHero;
        public bool IsAlive => DiminishingStats.TryGetValue(Stats.Health, out var health) && health.CurrentValue > 0;
        public int Id;
        public Team Team { get; private set;}
        public Vector2Int Position { get; set; }
        public Vector2Int Size => new(1, 1);
        public int Speed => DiminishingStats.TryGetValue(Stats.Stamina, out var stamina) && stamina.CurrentValue > 0 
            ? CurrentStats.GetStatInt(Stats.Speed) : 0;
        public StatsContainer CurrentStats { get; set; }
        public Dictionary<string, DiminishingStat> DiminishingStats = new ();
        public List<SkillData> Skills = new ();
        public List<Perk> Perks = new ();
        public BattleController Controller { get; }
        public PrivateCharacterEvents Events { get; private set; } = new();

        public Character(FighterData data, Team team, BattleController controller, int id)
        {
            Data = data;
            Team = team;
            Controller = controller;
            Id = id;

            InitializeStats();
            InitializePerks();
        }

        public Character(Hero data, Team team, BattleController controller)
        {
            Data = data.FighterData;
            CurrentStats = data.CurrentStats.CreateRuntimeCopy();
            Team = team;
            Controller = controller;

            InitializeStats();
            InitializePerks();
        }
        
        private void InitializePerks()
        {
            foreach (PerkData perkData in Data.Perks)
                Perks.Add(new Perk(perkData, this));
        }

        // TODO: WTF is that???????
        private void InitializeStats()
        {
            CurrentStats = Data.DefaultStats.CreateRuntimeCopy();
            foreach (var statRecord in CurrentStats.records.records)
            {
                if (!(statRecord.Definition is StatData statData) || !statData.Diminishing)
                {
                    DiminishingStat dimStat = new DiminishingStat(statRecord);
                    DiminishingStats.Add(statRecord.Definition.Id, dimStat);
                    CurrentStats.OnStatChangeSubscribe(statRecord, dimStat.StatDiminished);
                }
            }
        }
    }

    public class PrivateCharacterEvents
    {
        public Action<int> DamageTaken;
        public Action<Vector2Int> Moved;
        public Action<BattleAnimation> Attacked;
    }

    public enum Team
    {
        None = 0,
        Player = 1,
        Enemy = 2
    }
}