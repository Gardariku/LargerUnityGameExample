using System.Collections;
using Battle.Model;

namespace Battle.Controller
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

            yield break;
        }

        public BattleLoop(BattleController controller) : base(controller)
        {
        }
    }
}