using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class KillCommand : IBattleCommand
    {
        public Character Target { get; private set; }

        public KillCommand(Character target)
        {
            Target = target;
        }
        
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.CharacterEvents.CharacterDeathFinished?.Invoke(Target);
            controller.CheckWinCondition(Target);
        }
    }
}