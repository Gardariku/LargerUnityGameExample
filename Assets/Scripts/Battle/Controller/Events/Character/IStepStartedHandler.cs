using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IStepStartedHandler : IEventSubscriber
    {
        public void OnStepStarted(StepCommand stepCommand);
    }
}