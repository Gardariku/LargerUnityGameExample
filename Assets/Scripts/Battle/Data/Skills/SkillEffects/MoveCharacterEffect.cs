using System;
using Battle.Controller;
using Battle.Controller.Commands;
using Battle.Controller.GameplayLoops;

namespace Battle.Data.Skills.SkillEffects
{
    [Serializable, AddTypeMenu("Common / Move")]
    public class MoveCharacterEffect : SkillEffect
    {
        public override void Apply(BattleController controller, ActionModel data)
        {
            var path = controller.BattleModel.Field.FindPath(data.Actor.Position, data.MainCell);
            controller.AddCommandMainLast(new MoveCharacterCommand(data.Actor, path));
        }
    }
}