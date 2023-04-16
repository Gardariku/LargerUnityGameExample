using System.Collections;

namespace Battle.Controller.GameplayLoops
{
    public class BattleLoop : GameplayLoop<BattleModel>
    {
        public override IEnumerator Start()
        {
            while (Model.Winner == Team.None)
            {
                _controller.GameStateEvents.GameStarted?.Invoke();
                yield return _controller.RoundLoop.Start();
            }

            _controller.GameStateEvents.GameEnded?.Invoke(Model.Winner);
        }

        public BattleLoop(BattleController controller) : base(controller)
        {
        }
    }
}