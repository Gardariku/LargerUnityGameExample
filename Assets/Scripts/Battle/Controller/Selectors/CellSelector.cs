using System;
using UnityEngine;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("Cell Selection")]
    public class CellSelector : ITargetSelector
    {
        [field: SerializeField] public AreaOfEffect Area { get; private set; }
        [field: SerializeField] public int Magnitude { get; private set; }
        [field: SerializeField] public EffectImpact Impact { get; private set; }
        
        public CellSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor)
        {
            controller.GameStateEvents.TargetSelectionStarted?.Invoke(this, actor);
        }
    }
}