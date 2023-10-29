using System.Collections.Generic;
using Battle.Controller.Events.Character;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class KillCommand : IBattleCommand
    {
        public Character Target { get; private set; }

        public KillCommand(Character target)
        {
            Target = target;
        }
        
        public void Execute(BattleController controller)
        {
            controller.EventBus.RaiseEvent<IDeathHandler>(handler => handler.OnDeath(Target));
            Debug.Log($"{Target.Data.Name} character died.");
            controller.CheckWinCondition(Target);
        }
    }
}