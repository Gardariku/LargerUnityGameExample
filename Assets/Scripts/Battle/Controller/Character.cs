using System;
using System.Collections.Generic;
using Battle.Controller;
using Battle.Controller.Commands;
using Battle.Controller.Field;
using Battle.Data;
using Battle.Model.Characters;
using Battle.Model.Perks;
using Battle.Model.Skills;
using Battle.Model.Stats;
using CleverCrow.Fluid.StatsSystem;
using CleverCrow.Fluid.StatsSystem.StatsContainers;
using UnityEditor.Animations;
using UnityEngine;

namespace Battle.Model
{
    [Serializable]
    public class Character : IFieldObject
    {
        [field: SerializeField] public FighterData Data { get; private set; }
        public bool IsHero => Data.IsHero;
        public bool IsAlive => DiminishingStats.TryGetValue("HEALTH", out var health) && health.CurrentValue > 0;
        public int Id;
        public Team Team { get; private set;}
        public Vector2Int Position { get; set; }
        public Vector2Int Size => new(1, 1);
        public int Speed => DiminishingStats.TryGetValue("STAMINA", out var stamina) && stamina.CurrentValue > 0 
            ? CurrentStats.GetStatInt("SPEED") : 0;
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

        public void Move(Vector2Int newPos)
        {
            Position = newPos;
            Events.Moved?.Invoke(newPos);
        }

        // TODO: Add death check and event?
        // Ok, maybe this stat system is kinda strange
        public void TakeDamage(int damage)
        {
            if (DiminishingStats.TryGetValue("HEALTH", out var health))
            {
                health.CurrentValue = Mathf.Clamp(health.CurrentValue - damage, 0, health.CurrentValue);
                //Controller.CharacterEvents.CharacterDiminishingStatChanged?.Invoke(this, health);
                Events.DamageTaken?.Invoke(damage);
                if (!IsAlive)
                    Controller.AddCommandMainLast(new KillCommand(this));
                Debug.Log(Data.Name + " has taken " + damage + " damage and now has " + health.CurrentValue + "hp");
            }
        }

        public void RestoreHealth(int heal)
        {
            if (DiminishingStats.TryGetValue("HEALTH", out var health))
            {
                health.CurrentValue = Mathf.Clamp(health.CurrentValue + heal, 0, health.MainStat.GetValueInt());
                //Controller.CharacterEvents.CharacterDiminishingStatChanged(this, health);
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