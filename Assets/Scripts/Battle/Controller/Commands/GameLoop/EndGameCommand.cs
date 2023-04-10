using System.Collections.Generic;
using Battle.Model;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class EndGameCommand : IBattleCommand
    {
        public void Execute(BattleController controller, BattleModel model)
        {
            Debug.Assert(model.Winner != Team.None);
            controller.GameStateEvents.GameEnded?.Invoke(model.Winner);
        }
    }
}