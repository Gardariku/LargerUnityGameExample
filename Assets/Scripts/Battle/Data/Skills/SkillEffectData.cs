using System;
using System.Collections.Generic;
using Battle.Controller;
using UnityEngine;

namespace Battle.Model.Skills
{
    [Serializable]
    public abstract class SkillEffectData
    {
        [SerializeField]
        private SkillData Skill;
        [SerializeField]
        private string ID;
    
        public abstract void Apply(BattleController controller, Character user, List<Character> target);
    }
}
