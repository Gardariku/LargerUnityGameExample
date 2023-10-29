using System.Collections;
using System.Collections.Generic;
using Battle.Controller.Events.Character;
using UnityEngine;

namespace Battle.Controller.GameplayLoops
{
    public class ActionLoop : GameplayLoop<ActionModel>
    {
        private Character _actor;
        
        public ActionLoop(BattleController controller) : base(controller)
        {
            Model = new ActionModel();
        }

        public override IEnumerator Start()
        {
            _controller.EventBus.RaiseEvent<IActionStartedHandler>(handler => handler.OnActionStarted());
            yield return null;
            
            _controller.EventBus.RaiseEvent<IActionFinishedHandler>(handler => handler.OnActionFinished());
            Model.Reset();
        }
    }

    public class ActionModel
    {
        public Character Actor;
        public Character MainTarget => AffectedCharacters[0];
        public Vector2Int MainCell => AffectedCells[0];
        public List<Character> AffectedCharacters = new ();
        public List<Vector2Int> AffectedCells = new ();

        public void Reset()
        {
            Actor = null;
            AffectedCharacters.Clear();
            AffectedCells.Clear();
        }
    }
}