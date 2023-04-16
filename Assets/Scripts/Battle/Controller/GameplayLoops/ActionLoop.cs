using System.Collections;
using System.Collections.Generic;
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
            Model = new ActionModel();
            _controller.CharacterEvents.CharacterActionStarted?.Invoke();
            yield return null;
            
            _controller.CharacterEvents.CharacterActionFinished?.Invoke();
        }
    }

    public class ActionModel
    {
        public Character Actor;
        public Character MainTarget;
        public List<Character> AffectedCharacters = new ();
        public Vector2Int MainCell = new (-1, -1);
        public List<Vector2Int> AffectedCells = new ();

        public void Reset()
        {
            Actor = null;
            MainTarget = null;
            AffectedCharacters.Clear();
            AffectedCells.Clear();
            MainCell = new(-1, -1);
        }
    }
}