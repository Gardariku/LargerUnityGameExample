using System;
using Battle.Controller.Events.Input;
using Battle.Data.Skills;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("No Selection")]
    public class NoneSelector : ITargetSelector
    {
        public NoneSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor, SkillData skill)
        {
            controller.EventBus.RaiseEvent<ITargetSelectionStartedHandler>(
                handler => handler.OnTargetSelectionStarted(skill, actor));
        }
    }
}