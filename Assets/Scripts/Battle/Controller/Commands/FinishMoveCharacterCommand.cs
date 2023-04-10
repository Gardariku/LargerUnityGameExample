using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class FinishMoveCharacterCommand : IBattleCommand
    {
        public MoveCharacterCommand Move { get; private set; }

        public FinishMoveCharacterCommand(MoveCharacterCommand moveCommand)
        {
            Move = moveCommand;
        }
        
        public void Execute(BattleController controller, BattleModel model)
        {
            Debug.Log($"Moved character {Move.Actor.Data.Name} to {Move.Path[Move.Path.Length - 1]}");
            controller.CharacterEvents.CharacterMoveFinished?.Invoke(Move);
        }
    }
}