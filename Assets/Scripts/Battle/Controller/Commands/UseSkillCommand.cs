using System.Collections.Generic;
using Battle.Data.Skills;

namespace Battle.Controller.Commands
{
    public class UseSkillCommand : IBattleCommand
    {
        private SkillData skill;
        private Character user;
        private List<Character> target;
        
        public UseSkillCommand(SkillData skill, Character user, List<Character> target)
        {
            this.skill = skill;
            this.user = user;
            this.target = target;
        }

        public void Execute(BattleController controller, BattleModel model)
        {
            controller.CharacterEvents.CharacterAbilityUsageStarted?.Invoke(this);
            skill.Use(controller, user, target);
            controller.CharacterEvents.CharacterAbilityUsageFinished?.Invoke(this);
        }
    }
}