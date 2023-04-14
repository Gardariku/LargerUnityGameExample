using System.Collections.Generic;
using Battle.Controller;
using Battle.Data;
using UnityEngine;

namespace Battle.Model.Skills
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Build/Skills/Skill")]
    public class SkillData : ScriptableObject
    {
        [Header("Info")]
        public string ID;
        public string Name;
        public Sprite Icon;
        public string Description;
        
        [field: Space]
        [field: SerializeReference, SubclassSelector] public SkillEffectData[] Effects { get; private set; }
        
        [Header("Visuals")]
        public BattleAnimation UsageAnimation;

        public void Use(BattleController controller, Character user, List<Character> target)
        {
            foreach (var effect in Effects)
                effect.Apply(controller, user, target);
        }
    }
}
