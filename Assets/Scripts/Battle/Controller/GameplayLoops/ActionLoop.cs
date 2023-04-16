using System.Collections;

namespace Battle.Controller.GameplayLoops
{
    public class ActionLoop : GameplayLoop<ActionModel>
    {
        private Character _actor;
        
        public ActionLoop(BattleController controller) : base(controller)
        {
        }

        public override IEnumerator Start()
        {
            Model = new ActionModel();
            _controller.CharacterEvents.CharacterActionStarted?.Invoke();
            yield return null;
            
            _controller.CharacterEvents.CharacterActionFinished?.Invoke();
        }
    }

    public struct ActionModel
    {
        public int Data;
    }
}