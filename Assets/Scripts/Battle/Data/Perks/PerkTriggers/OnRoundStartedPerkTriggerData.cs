using System;
using Battle.Controller;

namespace Battle.Data.Perks.PerkTriggers
{
    [Serializable]
    public class OnRoundStartedPerkTriggerData : PerkTriggerData
    {
        public override void Subscribe(Perk perk, Character character)
        {
            character.Controller.GameStateEvents.RoundStarted += round => perk.OnTrigger(this, 1, null);
        }
    }
}