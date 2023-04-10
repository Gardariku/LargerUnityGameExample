using CleverCrow.Fluid.StatsSystem.StatsContainers;
using UnityEngine;

namespace Battle.Model.Items
{
    [CreateAssetMenu(fileName = "New Artifact", menuName = "Build/Artifact")]
    public class ArtifactData : ScriptableObject
    {
        public int ID;
        public string Name;
        public EquipmentSlot Slot;
        public Sprite Icon;
        public string Description;
        public StatsContainer Stats;
        // TODO: Add skills or perks?
    }

    public enum EquipmentSlot
    {
        RightHand = 0,
        LeftHand = 1,
        Helm = 2,
        Chest = 3,
        Feet = 4,
        Trinket = 5
    }
}