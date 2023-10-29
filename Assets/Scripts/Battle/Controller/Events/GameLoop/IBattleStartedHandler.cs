using Common.DataStructures;

namespace Battle.Controller.Events.GameLoop
{
    public interface IBattleStartedHandler : IEventSubscriber
    {
        public void OnBattleStarted();
    }
}