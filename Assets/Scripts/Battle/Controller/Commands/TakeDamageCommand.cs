using Battle.Model;

namespace Battle.Controller.Commands
{
    public class TakeDamageCommand : IBattleCommand
    {
        private Character victim;
        private int damage;

        public void Execute(BattleController controller, BattleModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}