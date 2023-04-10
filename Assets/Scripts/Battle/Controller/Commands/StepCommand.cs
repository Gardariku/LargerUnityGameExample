using System.Collections.Generic;
using Battle.Model;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class StepCommand : IBattleCommand
    {
        public Character Actor { get; private set; }
        public Vector2Int Start { get; set; }
        public Vector2Int Finish { get; set; }

        public StepCommand(Character character, Vector2Int start, Vector2Int finish)
        {
            Actor = character;
            Start = start;
            Finish = finish;
        }
        
        public void Execute(BattleController controller, BattleModel model)
        {
            controller.CharacterEvents.CharacterStepStarted?.Invoke(this);
            Actor.Move(Finish);
            controller.CharacterEvents.CharacterStepFinished?.Invoke(this);
        }
    }
}