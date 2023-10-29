using System.Collections;
using Battle.Controller.Events.GameLoop;

namespace Battle.Controller.GameplayLoops
{
    public class BattleLoop : GameplayLoop<BattleModel>
    {
        public override IEnumerator Start()
        {
            while (Model.Winner == Team.None)
            {
                _controller.EventBus.RaiseEvent<IBattleStartedHandler>(handler => handler.OnBattleStarted());
                yield return _controller.RoundLoop.Start();
            }

            _controller.EventBus.RaiseEvent<IBattleFinishedHandler>(handler => handler.OnBattleFinished(Model.Winner));
        }

        public BattleLoop(BattleController controller) : base(controller)
        {
        }
    }
}