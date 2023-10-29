using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Effects
{
    public interface IDiminishingStatChangedHandler : IEventSubscriber
    {
        public void OnDiminishingStatChanged(ChangeDimStatCommand changeDimStatCommand);
    }
}