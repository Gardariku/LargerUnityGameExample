using System;
using Battle.Controller;
using Battle.Controller.Commands;
using Battle.Controller.GameplayLoops;
using UnityEngine;

namespace Battle.Data.Skills.SkillEffects
{
    [Serializable, AddTypeMenu("Common / Attack")]
    public class AttackEffect : SkillEffect
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; } = DamageType.Physical;
        
        public override void Apply(BattleController controller, ActionModel data)
        {
            controller.AddCommandMainLast(new AttackCommand(data.Actor, data.MainTarget));
        }
    }
}