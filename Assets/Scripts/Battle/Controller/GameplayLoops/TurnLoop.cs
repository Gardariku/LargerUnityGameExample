using System.Collections;
using Battle.Model;
using UnityEngine;

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

                yield return new WaitWhile(() => Model.WaitingForInput);
            }
            else
            {
                foreach (var action in AI.CalculateTurn(currentCharacter, _controller))
                {
                    _controller.AddLoop(_controller.ActionLoop.Start());
                    _controller.AddCommandMainLast(action);
                    yield return null;
                }
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