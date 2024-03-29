using System.Collections.Generic;
using Battle.Controller.Events.Effects;
using Battle.Data.Stats;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class RestoreHealthCommand : IBattleCommand
    {
        public Character Healer { get; }
        public Character Recipient { get; }
        public int Heal { get; }
        
        public RestoreHealthCommand(Character healer, Character recipient, int heal)
        {
            Healer = healer;
            Recipient = recipient;
            Heal = heal;
        }

        public void Execute(BattleController controller)
        {
            controller.EventBus.RaiseEvent<IHealStartedHandler>(handler => handler.OnHealStarted(this));
            //CalculateModifiers();
            if (Recipient.DiminishingStats.TryGetValue(Stats.Health, out var health))
            {
                health.CurrentValue = Mathf.Clamp(health.CurrentValue + Heal, 0, health.MainStat.GetValueInt());
                //Controller.CharacterEvents.CharacterDiminishingStatChanged(this, health);
            }
            controller.EventBus.RaiseEvent<IHealFinishedHandler>(handler => handler.OnHealFinished(this));
        }

        private void CalculateModifiers()
        {
            throw new System.NotImplementedException();
        }
    }
}