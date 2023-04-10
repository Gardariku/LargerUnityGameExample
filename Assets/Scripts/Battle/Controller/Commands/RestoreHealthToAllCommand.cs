using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class RestoreHealthToAllCommand : IBattleCommand
    {
        public Character Healer { get; }
        public List<Character> Recipients { get; }
        public int Heal { get; }
        
        public RestoreHealthToAllCommand(Character healer, List<Character> recipients, int heal)
        {
            Healer = healer;
            Recipients = recipients;
            Heal = heal;
        }

        public void Execute(BattleController controller, BattleModel model)
        {
            foreach (var recipient in Recipients)
                controller.AddCommandBatchFirst(new RestoreHealthCommand(Healer, recipient, Heal));
        }
    }
}