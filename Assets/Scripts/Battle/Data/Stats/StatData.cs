using CleverCrow.Fluid.StatsSystem;
using UnityEngine;

namespace Battle.Data.Stats
{
    [CreateAssetMenu(fileName = "New Stat Data", menuName = "Build/Stats/Stat", order = 0)]
    public class StatData : StatDefinition
    {
        public Sprite Icon;
        public bool Diminishing;

        public void Subscribe()
        {
            throw new System.NotImplementedException();
        }
    }
}