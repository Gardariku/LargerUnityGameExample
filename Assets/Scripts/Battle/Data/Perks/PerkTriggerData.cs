using System;
using UnityEngine;

namespace Battle.Model.Perks
{
    [Serializable]
    public abstract class PerkTriggerData
    {
        [field: SerializeField] public int Threshold { get; private set; } = 1;

        public abstract void Subscribe(Perk perk, Character character);
    }
}