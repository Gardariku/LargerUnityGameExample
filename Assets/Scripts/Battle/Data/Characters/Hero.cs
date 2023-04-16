using System.Collections.Generic;
using Battle.Data.Items;
using Battle.Data.Perks;
using Battle.Data.Skills;
using CleverCrow.Fluid.StatsSystem.StatsContainers;

namespace Battle.Data.Characters
{
    public class Hero
    {
        public HeroData HeroData { get; }
        public FighterData FighterData { get; }
        public StatsContainer CurrentStats { get; }
        
        public List<SkillData> AcquiredSkills { get; } = new List<SkillData>();
        public List<PerkData> AcquiredPerks { get; } = new List<PerkData>();
        public List<ArtifactData> EquippedArtifacts { get; } = new List<ArtifactData>();
        
        public Hero(HeroData heroData, FighterData fighterData)
        {
            HeroData = heroData;
            FighterData = fighterData;
            CurrentStats = FighterData.DefaultStats.CreateRuntimeCopy();
        }
    }
}