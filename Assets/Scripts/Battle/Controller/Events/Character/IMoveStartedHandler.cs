using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IMoveStartedHandler : IEventSubscriber
    {
        public void OnMoveStarted(MoveCharacterCommand moveCommand);
    }
}