using System;
using Battle.Controller;
using UnityEngine;

namespace Battle.Data.Perks
{
    [Serializable]
    public abstract class PerkTriggerData
    {
        [field: SerializeField] public int Threshold { get; private set; } = 1;

        public abstract void Subscribe(Perk perk, Character character);
    }
}