using System;
using UnityEngine;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("Character Selection")]
    public class CharacterSelector : ITargetSelector
    {
        [field: SerializeField] public CharacterTarget TargetRequirements { get; private set; }
        [field: SerializeField] public AreaOfEffect Area { get; private set; }
        [field: SerializeField] public int Magnitude { get; private set; }
        [field: SerializeField] public EffectImpact Impact { get; private set; }
        
        public CharacterSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor)
        {
            controller.GameStateEvents.TargetSelectionStarted?.Invoke(this, actor);
        }
        
        public enum CharacterTarget
        {
            Any,
            Friendly,
            Enemy
        }
    }
}