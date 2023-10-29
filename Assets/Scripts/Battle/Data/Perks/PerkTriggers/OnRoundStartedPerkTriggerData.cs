using System;
using Battle.Controller;
using Battle.Controller.Events.GameLoop;

namespace Battle.Data.Perks.PerkTriggers
{
    [Serializable]
    public class OnRoundStartedPerkTriggerData : PerkTriggerData, IRoundStartedHandler
    {
        private Perk _perk;
        private Character _character;
        
        public override void Subscribe(Perk perk, Character character)
        {
            _perk = perk;
            _character = character;
            character.Controller.EventBus.Subscribe(this);
        }

        public void OnRoundStarted(int round)
        {
            _perk.OnTrigger(this, 1, null);
        }
    }
}