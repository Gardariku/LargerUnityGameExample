using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IMoveFinishedHandler : IEventSubscriber
    {
        public void OnMoveFinished(FinishMoveCharacterCommand finishMoveCommand);
    }
}