using System.Collections.Generic;
using Battle.Controller.Events.Character;
using Battle.Data.Skills;

namespace Battle.Controller.Commands
{
    public class UseSkillCommand : IBattleCommand
    {
        public SkillData Skill {get; private set; }
        public Character User {get; private set; }
        
        public UseSkillCommand(SkillData skill, Character user)
        {
            Skill = skill;
            User = user;
        }

        public void Execute(BattleController controller)
        {
            controller.EventBus.RaiseEvent<IAbilityUsageStartedHandler>(handler => handler.OnAbilityUsageStarted(this));
            Skill.Use(controller, User);
            controller.EventBus.RaiseEvent<IAbilityUsageFinishedHandler>(handler => handler.OnAbilityUsageFinished(this));
        }
    }
}