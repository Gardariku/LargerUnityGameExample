using UnityEngine;

namespace Battle.Controller.Commands
{
    public class TakeDamageCommand : IBattleCommand
    {
        public Character Victim { get; set; }
        public int Damage  { get; set; }

        public TakeDamageCommand(Character victim, int damage)
        {
            Victim = victim;
            Damage = damage;
        }

        public void Execute(BattleController controller)
        {
            var victimStats = Victim.DiminishingStats;
            if (victimStats.TryGetValue(Stats.Health, out var health))
            {
                health.CurrentValue = Mathf.Clamp(health.CurrentValue - Damage, 0, health.CurrentValue);
                //Controller.CharacterEvents.CharacterDiminishingStatChanged?.Invoke(this, health);
                Victim.Events.DamageTaken?.Invoke(Damage);
                if (!Victim.IsAlive)
                    controller.AddCommandMainLast(new KillCommand(Victim));
                Debug.Log(Victim.Data.Name + " has taken " + Damage + " damage and now has " + health.CurrentValue + "hp");
            }
        }
    }
}