using System.Collections.Generic;
using Battle.Controller.Events.Character;
using Battle.Data;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class AttackCommand : IBattleCommand
    {
        public Character Attacker { get; private set; }
        public Character Target { get; private set; }
        public DealDamageCommand DamageCommand { get; private set; }
        
        public AttackCommand(Character attacker, Character target)
        {
            Attacker = attacker;
            Target = target;
        }
        
        public void Execute(BattleController controller)
        {
            controller.EventBus.RaiseEvent<IAttackStartedHandler>(handler => handler.OnAttackStarted(this));
            
            Attacker.Events.Attacked?.Invoke(BattleAnimation.Attack);
            DamageCommand = new DealDamageCommand(Attacker, Target,
                Attacker.CurrentStats.GetStatInt("DAMAGE"), DamageType.Physical);
            controller.AddCommandMainLast(DamageCommand);

            controller.EventBus.RaiseEvent<IAttackFinishedHandler>(handler => handler.OnAttackFinished(this));
        }
    }
}