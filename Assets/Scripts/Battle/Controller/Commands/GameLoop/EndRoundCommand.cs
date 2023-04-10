using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class EndRoundCommand : IBattleCommand
    {
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.GameStateEvents.RoundEnded?.Invoke();
            controller.AddCommandMainLast(new StartRoundCommand());
        }
    }
}