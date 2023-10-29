using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IAttackStartedHandler : IEventSubscriber
    {
        public void OnAttackStarted(AttackCommand attackCommand);
    }
}