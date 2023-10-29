using Battle.Data.Skills;
using Common.DataStructures;

namespace Battle.Controller.Events.Input
{
    public interface ITargetSelectionFinishedHandler : IEventSubscriber
    {
        public void OnTargetSelectionFinished(SkillData skill, Controller.Character character);

    }
}