using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Effects
{
    public interface IDamageFinishedHandler : IEventSubscriber
    {
        public void OnDamageFinished(DealDamageCommand damageCommand);
    }
}