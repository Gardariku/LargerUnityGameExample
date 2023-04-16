using System.Collections;
using System.Collections.Generic;
using Battle.Controller.Commands;

namespace Battle.Controller.GameplayLoops
{
    public class RoundLoop : GameplayLoop<RoundModel>
    {
        public RoundLoop(BattleController controller) : base(controller)
        {
            Model = new RoundModel();
        }
        
        public override IEnumerator Start()
        {
            _controller.GameStateEvents.RoundStarted?.Invoke(_controller.BattleModel.Round++);
            DetermineTurnOrder();
            Model.CurrentTurn = 0;

            var characters = _controller.BattleModel.Characters;
            var restoreStaminaCommands = new IBattleCommand[characters.Count];
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].DiminishingStats.TryGetValue("STAMINA", out var stamina))
                    restoreStaminaCommands[i] = (new ChangeDimStatCommand(characters[i], stamina, 
                        characters[i].CurrentStats.GetStatInt("STAMINA")));
            }
            _controller.AddCommandMainLast(restoreStaminaCommands);

            while (Model.CurrentTurn < Model.TurnOrder.Count)
            {
                yield return _controller.TurnLoop.Start();
                Model.CurrentTurn++;
                
                if (_controller.RoundModel.CurrentTurn > Model.TurnOrder.Count)
                    throw new System.ArgumentException("Current model has " + _controller.RoundModel.TurnOrder.Count 
                        + " characters with their own turn but current turn index is " + Model.CurrentTurn);
            }
            
            _controller.GameStateEvents.RoundEnded?.Invoke();
            yield return null;
        }

        public void DetermineTurnOrder()
        {
            Model.TurnOrder = new List<Character>(_controller.BattleModel.Characters);
            foreach (var character in Model.TurnOrder)
            {
                if (character.CurrentStats.GetStat("INIT") == 0f || !character.IsAlive)
                    Model.TurnOrder.Remove(character);
            }
            Model.TurnOrder.Sort((x, y) =>
                x.CurrentStats.GetStat("INIT").CompareTo(y.CurrentStats.GetStat("INIT")));
        }
    }

    public class RoundModel
    {
        public List<Character> TurnOrder = new List<Character>();
        public int CurrentTurn;
    }
}