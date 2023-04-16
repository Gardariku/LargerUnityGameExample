using System;
using System.Collections.Generic;
using Battle.Controller;
using Battle.Controller.GameplayLoops;
using UnityEngine;

namespace Battle.Data.Skills
{
    // Effects create and queue their corresponding commands for execution,
    // based on pre-filled information (such as skill targets) in current ActionLoop data model.
    // All effects use the same models field as their input, but can interpret it in their specific way.
    // Effect doesn't hold actual battle logic and instead serves as interpreter for BattleCommands.
    [Serializable]
    public abstract class SkillEffect
    {
        public abstract void Apply(BattleController controller, ActionModel data);
    }
}
