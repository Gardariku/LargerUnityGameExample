using Battle.Data.Skills;
using Common.DataStructures;

namespace Battle.Controller.Events.Input
{
    public interface ITargetSelectionStartedHandler : IEventSubscriber
    {
        public void OnTargetSelectionStarted(SkillData skill, Controller.Character character);
    }
}