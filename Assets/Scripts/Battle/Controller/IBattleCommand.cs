namespace Battle.Controller
{
    public interface IBattleCommand
    {
        public void Execute(BattleController controller, BattleModel model);
    }
}