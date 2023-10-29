using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IActionFinishedHandler : IEventSubscriber
    {
        public void OnActionFinished();
    }
}