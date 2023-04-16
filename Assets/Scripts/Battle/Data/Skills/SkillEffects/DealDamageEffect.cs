using System;
using Battle.Controller;
using Battle.Controller.Commands;
using Battle.Controller.GameplayLoops;
using UnityEngine;

namespace Battle.Data.Skills.SkillEffects
{
    [Serializable, AddTypeMenu("Common / Deal Damage")]
    public class DealDamageEffect : SkillEffect
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Magical;

        public override void Apply(BattleController controller, ActionModel data)
        {
            controller.AddCommandMainFirst(new DealDamageCommand(data.Actor, data.MainTarget, 
                Damage, DamageType));
        }
    }
}
