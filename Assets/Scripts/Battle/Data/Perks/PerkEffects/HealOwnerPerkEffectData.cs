using System;
using Battle.Controller;
using Battle.Controller.Commands;
using UnityEngine;

namespace Battle.Data.Perks.PerkEffects
{
    [Serializable]
    public class HealOwnerPerkEffectData : PerkEffectData
    {
        [field: SerializeField]
        public int Heal { get; private set; }
        
        public override void Proc(Character character, IBattleCommand command)
        {
            character.Controller.AddCommandMainLast(new RestoreHealthCommand(character, character, Heal));
        }
    }
}