using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IAbilityUsageStartedHandler : IEventSubscriber
    {
        public void OnAbilityUsageStarted(UseSkillCommand skillCommand);
    }
}