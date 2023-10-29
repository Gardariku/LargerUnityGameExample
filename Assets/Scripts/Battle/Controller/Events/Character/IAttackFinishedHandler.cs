using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IAttackFinishedHandler : IEventSubscriber
    {
        public void OnAttackFinished(AttackCommand attackCommand);
    }
}