using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IDeathHandler : IEventSubscriber
    {
        public void OnDeath(Controller.Character character);
    }
}