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
                controller.AddCommandMainLast(AI.CalculateTurn(currentCharacter, controller));
            }
            //return new List<BattleCommand> {new EndTurnCommand()};
        }
    }
}