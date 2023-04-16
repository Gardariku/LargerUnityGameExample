using System.Collections.Generic;
using Battle.Data.Stats;
using UnityEngine;

namespace Battle.Controller.Commands
{
    public class ChangeDimStatCommand : IBattleCommand
    {
        public Character Character;
        public DiminishingStat Stat;
        public int NewValue;
        public int PreviousValue;

        public ChangeDimStatCommand(Character character, DiminishingStat stat, int newValue)
        {
            Character = character;
            Stat = stat;
            NewValue = newValue;
        }

        public void Execute(BattleController controller, BattleModel model)
        {
            PreviousValue = Stat.CurrentValue;
            Stat.CurrentValue = NewValue;
            controller.CharacterEvents.CharacterDiminishingStatChanged?.Invoke(this);
        }
    }
}