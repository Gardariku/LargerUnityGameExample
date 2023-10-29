using System;
using System.Collections.Generic;
using Battle.Controller.Events.Character;
using Battle.Controller.Field;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class MoveCharacterCommand : IBattleCommand
    {
        public Character Actor { get; private set; }
        public Vector2Int[] Path { get; private set; }

        public MoveCharacterCommand(Character actor, Vector2Int[] path)
        {
            Path = path;
            Actor = actor;
        }
        
        // This command may have to be split into sub-steps, but rn i don't feel like it
        public void Execute(BattleController controller)
        {
            BattleField field = controller.BattleModel.Field;
            controller.EventBus.RaiseEvent<IMoveStartedHandler>(handler => handler.OnMoveStarted(this));
            int index = -1;
            
            foreach (var cell in Path)
            {
                if (++index == 0) continue;
                StepCommand step = new StepCommand(Actor, Path[index - 1], Path[index]);
                if (field.Cells[cell.x, cell.y] != null)
                    throw new Exception($"Cell {cell} is supposed to be empty during move of character " + 
                    $"{Actor.Data.Name}, but is currently occupied with {field.Cells[cell.x, cell.y]}");
                
                controller.AddCommandMainLast(step);
            }
            
            controller.AddCommandMainLast(new FinishMoveCharacterCommand(this));
        }
    }
}