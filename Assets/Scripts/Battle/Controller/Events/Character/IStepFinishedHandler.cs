using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IStepFinishedHandler : IEventSubscriber
    {
        public void OnStepFinished(StepCommand stepCommand);
    }
}