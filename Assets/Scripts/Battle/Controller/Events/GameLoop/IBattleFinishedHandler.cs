using Common.DataStructures;

namespace Battle.Controller.Events.GameLoop
{
    public interface IBattleFinishedHandler : IEventSubscriber
    {
        public void OnBattleFinished(Team winner);
    }
}