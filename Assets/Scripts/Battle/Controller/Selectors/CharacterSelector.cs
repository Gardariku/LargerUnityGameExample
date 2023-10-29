using System;
using Battle.Controller.Events.Input;
using Battle.Data.Skills;
using UnityEngine;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("Character Selection")]
    public class CharacterSelector : ITargetSelector
    {
        [field: SerializeField] public int Range { get; private set; } = 1;
        [field: SerializeField] public CharacterTarget TargetRequirements { get; private set; }
        [field: SerializeField] public AreaOfEffect Area { get; private set; }
        [field: SerializeField] public int Magnitude { get; private set; } = 1;
        [field: SerializeField] public EffectImpact Impact { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; } = 1;
        
        public CharacterSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor, SkillData skill)
        {
            controller.EventBus.RaiseEvent<ITargetSelectionStartedHandler>(
                handler => handler.OnTargetSelectionStarted(skill, actor));
        }
        
        public enum CharacterTarget
        {
            Any,
            Friendly,
            Enemy
        }
    }
}