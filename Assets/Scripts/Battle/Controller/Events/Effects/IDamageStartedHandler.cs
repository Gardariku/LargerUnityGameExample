using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Effects
{
    public interface IDamageStartedHandler : IEventSubscriber
    {
        public void OnDamageStarted(DealDamageCommand damageCommand);
    }
}