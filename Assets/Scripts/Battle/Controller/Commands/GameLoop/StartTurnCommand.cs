using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class StartTurnCommand : IBattleCommand
    {
        public void Execute(BattleController controller, BattleModel model)
        {
            var currentCharacter = controller.RoundModel.TurnOrder[controller.RoundModel.CurrentTurn];
            controller.GameStateEvents.CharacterTurnStarted?.Invoke(currentCharacter);

            if (currentCharacter.Team != Team.Player)
            {
                foreach (var action in AI.CalculateTurn(currentCharacter, controller))
                {
                    controller.AddLoop(controller.ActionLoop.Start());
                    controller.AddCommandMainLast(action);
                }
            }
            //return new List<BattleCommand> {new EndTurnCommand()};
        }
    }
}