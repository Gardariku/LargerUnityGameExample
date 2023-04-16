using System;
using System.Collections.Generic;
using Battle.Controller;
using Battle.Controller.Commands;
using UnityEngine;

namespace Battle.Data.Skills.SkillEffects
{
    [Serializable]
    public class DealDamageEffectData : SkillEffectData
    {
        [field: SerializeField]
        public int Damage { get; private set; }
        [field: SerializeField]
        public DamageType DamageType { get; private set; }

        public override void Apply(BattleController controller, Character user, List<Character> target)
        {
            Debug.Assert(target.Count > 0);
            controller.AddCommandMainFirst(new DealDamageCommand(user, target[0], Damage, DamageType));
        }
    }
}
