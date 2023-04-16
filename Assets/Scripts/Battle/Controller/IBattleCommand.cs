namespace Battle.Controller
{
    // Battle command represent state of specific actions instance.
    // It can also contain logic of how to modify battle data to perform the action,
    // queue other commands if needed, call corresponding events
    // and it can be modified by other entities (such as statuses) reacting to these events.
    public interface IBattleCommand
    {
        public void Execute(BattleController controller);
    }
}