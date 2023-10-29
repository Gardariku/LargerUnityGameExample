using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface ITurnStartedHandler : IEventSubscriber
    {
        public void OnTurnStarted(Controller.Character character);
    }
}