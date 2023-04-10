using System.Collections.Generic;
using Battle.Model;
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
        
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.CharacterEvents.CharacterAttackStarted?.Invoke(this);
            
            DamageCommand = new DealDamageCommand(Attacker, Target,
                Attacker.CurrentStats.GetStatInt("DAMAGE"), DamageType.Physical);
            controller.AddCommandBatchLast(DamageCommand);
            

            controller.CharacterEvents.CharacterAttackFinished?.Invoke(this);
            if (!Target.IsAlive)
                controller.AddCommandMainLast(new KillCommand(Target));
        }
    }
}