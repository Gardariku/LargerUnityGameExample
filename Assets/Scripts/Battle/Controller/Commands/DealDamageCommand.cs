using System;
using System.Collections.Generic;
using Battle.Data;

namespace Battle.Controller.Commands
{
    public class DealDamageCommand : IBattleCommand
    {
        public Character Attacker { get; }
        public Character Victim { get; }
        public int Damage { get; }
        public DamageType DamageType { get; }
        
        public DealDamageCommand(Character attacker, Character victim, int damage, DamageType damageType)
        {
            Attacker = attacker;
            Victim = victim;
            Damage = damage;
            DamageType = damageType;
        }
        
        // TODO: Add modifiers collection and damage result calculations
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.CharacterEvents.CharacterDamageDealingStarted?.Invoke(this);
            //CalculateModifiers();
            controller.CharacterEvents.CharacterDamageDealingFinished?.Invoke(this);
            controller.AddCommandBatchLast(new TakeDamageCommand(Victim, Damage));
        }

        private void CalculateModifiers()
        {
            throw new NotImplementedException();
        }
    }
}