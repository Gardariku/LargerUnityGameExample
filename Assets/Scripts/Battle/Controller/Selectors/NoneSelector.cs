using System;
using Battle.Data.Skills;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("No Selection")]
    public class NoneSelector : ITargetSelector
    {
        public NoneSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor, SkillData skill)
        {
            controller.GameStateEvents.TargetSelectionStarted?.Invoke(skill, actor);
        }
    }
}