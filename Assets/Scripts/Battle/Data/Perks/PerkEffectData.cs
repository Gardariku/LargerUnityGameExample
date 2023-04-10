using System;
using Battle.Controller;
using UnityEngine;

namespace Battle.Model.Perks
{
    [Serializable]
    public abstract class PerkEffectData
    {
        [field: SerializeField]
        public PerkData Perk { get; private set; }
        [field: SerializeField]
        public string ID { get; private set; }
    
        public abstract void Proc(Character character, IBattleCommand command);
    }
}
