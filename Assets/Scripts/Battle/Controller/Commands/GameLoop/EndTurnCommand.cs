using System.Collections.Generic;
using Battle.Model;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class EndTurnCommand : IBattleCommand  
    {
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.GameStateEvents.CharacterTurnEnded?.Invoke(controller.RoundModel.TurnOrder[controller.RoundModel.CurrentTurn]);

            if (++controller.RoundModel.CurrentTurn < controller.RoundModel.TurnOrder.Count)
                controller.AddCommandMainLast(new StartTurnCommand());
            else if (controller.RoundModel.CurrentTurn == controller.RoundModel.TurnOrder.Count)
                controller.AddCommandMainLast(new EndRoundCommand());
            else
                throw new System.ArgumentException("Current model has " + controller.RoundModel.TurnOrder.Count 
                + " characters with their own turn but current turn index is " + controller.RoundModel.CurrentTurn);
        }
    }
}