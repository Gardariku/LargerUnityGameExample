using Common.DataStructures;

namespace Battle.Controller.Events.GameLoop
{
    public interface IRoundFinishedHandler : IEventSubscriber
    {
        public void OnRoundFinished();
    }
}