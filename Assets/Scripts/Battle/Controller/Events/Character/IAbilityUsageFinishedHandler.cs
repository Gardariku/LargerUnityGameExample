using Battle.Controller.Commands;
using Common.DataStructures;

namespace Battle.Controller.Events.Character
{
    public interface IAbilityUsageFinishedHandler : IEventSubscriber
    {
        public void OnAbilityUsageFinished(UseSkillCommand skillCommand);
    }
}