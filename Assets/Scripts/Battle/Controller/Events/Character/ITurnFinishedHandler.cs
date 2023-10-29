using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface ITurnFinishedHandler : IEventSubscriber
    {
        public void OnTurnFinished(Controller.Character character);
    }
}