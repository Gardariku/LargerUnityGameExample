using System.Collections.Generic;
using Battle.Data;

namespace Battle.Controller.Commands
{
    public class DealDamageToAllCommand : IBattleCommand
    {
        public Character Attacker { get; }
        public List<Character> Victims { get; }
        public int Damage { get; }
        public DamageType DamageType { get; }
        
        public DealDamageToAllCommand(Character attacker, List<Character> victims, int damage, DamageType damageType)
        {
            Attacker = attacker;
            Victims = victims;
            Damage = damage;
            DamageType = damageType;
        }
        
        public void Execute(BattleController controller)
        {
            foreach (var victim in Victims)
                controller.AddCommandBatchLast(new DealDamageCommand(Attacker, victim, Damage, DamageType));
        }
    }
}