using System.Collections.Generic;
using System.IO;
using Battle.Controller.Events.Character;
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
        
        public void Execute(BattleController controller)
        {
            Debug.Log($"Moved character {Move.Actor.Data.Name} to {Move.Path[Move.Path.Length - 1]}");
            controller.EventBus.RaiseEvent<IMoveFinishedHandler>(handler => handler.OnMoveFinished(this));
        }
    }
}