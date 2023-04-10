using System.Collections.Generic;
using Battle.Model;

namespace Battle.Controller.Commands
{
    public class RestoreHealthCommand : IBattleCommand
    {
        public Character Healer { get; }
        public Character Recipient { get; }
        public int Heal { get; }
        
        public RestoreHealthCommand(Character healer, Character recipient, int heal)
        {
            Healer = healer;
            Recipient = recipient;
            Heal = heal;
        }

        public void Execute(BattleController controller, BattleModel model)
        {
            controller.CharacterEvents.CharacterHealStarted?.Invoke(this);
            //CalculateModifiers();
            controller.CharacterEvents.CharacterHealFinished?.Invoke(this);
            Recipient.RestoreHealth(Heal);
        }

        private void CalculateModifiers()
        {
            throw new System.NotImplementedException();
        }
    }
}