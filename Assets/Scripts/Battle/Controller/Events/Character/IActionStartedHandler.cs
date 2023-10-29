using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IActionStartedHandler : IEventSubscriber
    {
        public void OnActionStarted();
    }
}