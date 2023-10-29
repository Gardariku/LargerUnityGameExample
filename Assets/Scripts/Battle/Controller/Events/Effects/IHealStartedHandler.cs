using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Effects
{
    public interface IHealStartedHandler : IEventSubscriber
    {
        public void OnHealStarted(RestoreHealthCommand healCommand);
    }
}