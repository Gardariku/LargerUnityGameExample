using System.Collections.Generic;
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
            controller.CharacterEvents.CharacterDeathFinished?.Invoke(Target);
            Debug.Log($"{Target.Data.Name} character died.");
            controller.CheckWinCondition(Target);
        }
    }
}