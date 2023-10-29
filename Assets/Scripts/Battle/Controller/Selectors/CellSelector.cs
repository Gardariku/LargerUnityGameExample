using System;
using Battle.Controller.Events.Input;
using Battle.Data.Skills;
using UnityEngine;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("Cell Selection")]
    public class CellSelector : ITargetSelector
    {
        [field: SerializeField] public int Range { get; private set; } = 1000;
        [field: SerializeField] public CellTarget Target { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; } = 1;
        [field: SerializeField] public AreaOfEffect Area { get; private set; }
        [field: SerializeField] public int Magnitude { get; private set; }
        [field: SerializeField] public EffectImpact Impact { get; private set; }
        
        public CellSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor, SkillData skill)
        {
            controller.EventBus.RaiseEvent<ITargetSelectionStartedHandler>(
                handler => handler.OnTargetSelectionStarted(skill, actor));
        }
    }

    public enum CellTarget
    {
        Any,
        Empty
    }
}