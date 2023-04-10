using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class StartRoundCommand : IBattleCommand
    {
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.GameStateEvents.RoundStarted?.Invoke(model.Round++);
            //controller.RoundLoop.DetermineTurnOrder();
            //controller.CurrentTurn = 0;
            
            foreach (var character in model.Characters)
            {
                if (character.DiminishingStats.TryGetValue("STAMINA", out var stamina))
                    controller.AddCommandMainLast(new ChangeDimStatCommand(character, stamina, 
                        character.CurrentStats.GetStatInt("STAMINA")));
            }
            controller.AddCommandMainLast(new StartTurnCommand());
        }
    }
}