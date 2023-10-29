using Common.DataStructures;

namespace Battle.Controller.Events.GameLoop
{
    public interface IRoundStartedHandler : IEventSubscriber
    {
        public void OnRoundStarted(int round);
    }
}