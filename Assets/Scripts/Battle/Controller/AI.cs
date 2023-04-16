using System;
using System.Collections.Generic;
using Battle.Controller.Commands;

namespace Battle.Controller
{
    public static class AI
    {
        // TODO: implement actual AI
        public static List<IBattleCommand> CalculateTurn(Character character, BattleController controller)
        {
            List<IBattleCommand> sequence = new List<IBattleCommand>();
            if (character.DiminishingStats.TryGetValue("STAMINA", out var stamina)) ;
            {
                for (int i = 0; i < stamina.CurrentValue; i++)
                    sequence.Add(new AttackCommand(character, FindRandomTarget(character, controller.BattleModel)));
            }
            return sequence;
        }

        public static Character FindRandomTarget(Character initiator, BattleModel model)
        {
            if (initiator.Team != Team.Player)
            {
                return model.Allies[UnityEngine.Random.Range(0, model.Allies.Count)];
            }
            else
            {
                return model.Enemies[UnityEngine.Random.Range(0, model.Enemies.Count)];
            }
        }
    }
}