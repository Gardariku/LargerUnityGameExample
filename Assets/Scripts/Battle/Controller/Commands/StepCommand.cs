using System.Collections.Generic;
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
        
        public void Execute(BattleController controller)
        {
            controller.CharacterEvents.CharacterStepStarted?.Invoke(this);
            Actor.Position = Finish;
            Actor.Events.Moved?.Invoke(Finish);
            controller.CharacterEvents.CharacterStepFinished?.Invoke(this);
        }
    }
}