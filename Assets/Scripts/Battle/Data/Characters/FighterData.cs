using System;
using System.Collections.Generic;
using Battle.Data.Perks;
using Battle.Data.Skills;
using CleverCrow.Fluid.StatsSystem.StatsContainers;
using UnityEngine;

namespace Battle.Data.Characters
{
    [CreateAssetMenu(fileName = "New Combatant", menuName = "Build/Characters/Combatant", order = 0)]
    public class FighterData : ScriptableObject
    {
        public string ID;
        public string Name;
        public Sprite Icon;
        public Sprite Sprite;
        public bool IsHero;
        public StatsContainer DefaultStats;
        public List<SkillData> Skills;
        public List<PerkData> Perks;
        public List<BattleAnimationClip> Animations;

        [Serializable]
        public class BattleAnimationClip
        {
            public BattleAnimation Name;
            public AnimationClip Animation;
        }
    }
}