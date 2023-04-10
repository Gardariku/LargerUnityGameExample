using System.Collections;
using Battle.Model;

namespace Battle.Controller
{
    public class TurnLoop : GameplayLoop<TurnModel>
    {
        public TurnLoop(BattleController controller) : base(controller)
        {
            Model = new TurnModel();
        }
        
        public override IEnumerator Start()
        {
            var currentCharacter = _controller.RoundModel.TurnOrder[_controller.RoundModel.CurrentTurn];
            _controller.GameStateEvents.CharacterTurnStarted?.Invoke(currentCharacter);

            if (currentCharacter.Team == Team.Player)
            {
                Model.WaitingForInput = true;

                while (Model.WaitingForInput)
                {
                    yield return null;
                }
            }
            else
            {
                _controller.AddCommandMainLast(AI.CalculateTurn(currentCharacter, _controller));
            }
            yield return null;

            _controller.GameStateEvents.CharacterTurnEnded?.Invoke(
                _controller.RoundModel.TurnOrder[_controller.RoundModel.CurrentTurn]);
            yield return null;
        }

        public void EndTurn()
        {
            Model.WaitingForInput = false;
        }
    }

    public class TurnModel
    {
        public bool WaitingForInput { get; set; }
    }
}