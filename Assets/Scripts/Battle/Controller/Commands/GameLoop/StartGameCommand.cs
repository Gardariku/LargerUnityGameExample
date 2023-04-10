using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class StartGameCommand : IBattleCommand
    {
        public void Execute(BattleController controller, BattleModel model)
        {
            model.Round = 0;
            controller.GameStateEvents.GameStarted?.Invoke();
            controller.AddCommandMainLast(new StartRoundCommand());
        }
    }
}