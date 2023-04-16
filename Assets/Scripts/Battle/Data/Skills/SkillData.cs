using System.Collections.Generic;
using Battle.Controller;
using UnityEngine;

namespace Battle.Data.Skills
{
    // Characters skill consists of some Selection type instance and a combination of SkillEffects.
    // To use a skill character usually must confirm availability of required resources
    // and choose its target based on Selector. Then all of the effects are applied to battle model by passing
    // their corresponding commands to execute.
    [CreateAssetMenu(fileName = "New Skill", menuName = "Battle/Skill")]
    public class SkillData : ScriptableObject
    {
        [field: Header("Info")]
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        
        [field: Space]
        [field: SerializeReference, SubclassSelector] public ITargetSelector Selection { get; private set; }
        [field: SerializeReference, SubclassSelector] public SkillEffect[] Effects { get; private set; }
        
        [Header("Visuals")]
        public BattleAnimation UsageAnimation;

        public void Use(BattleController controller, Character user)
        {
            var data = controller.ActionLoop.Model;
            data.Actor = user;
            foreach (var effect in Effects)
                effect.Apply(controller, data);
        }
    }
}
