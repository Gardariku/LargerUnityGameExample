using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Effects
{
    public interface IHealFinishedHandler : IEventSubscriber
    {
        public void OnHealFinished(RestoreHealthCommand healCommand);
    }
}