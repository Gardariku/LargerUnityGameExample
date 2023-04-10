using System.Linq;
using Battle.Controller;
using UnityEngine;

namespace Battle.Model.Perks
{
    public class Perk
    {
        public PerkData Data { get; }
        private int[] progress { get; }
        private Character owner;
        
        public Perk(PerkData data, Character owner)
        {
            this.owner = owner;
            Data = data;
            progress = new int[Data.Effects.Length];
            
            Data.Activate(this, owner);
        }

        public void OnTrigger(PerkTriggerData trigger, int addProgress, IBattleCommand command)
        {
            int index = Data.GetEffectID(trigger);
            Debug.Assert(index > 0 && index < Data.Effects.Length);
            int fullProgress = addProgress + progress[index];
            int procs = fullProgress / Data.Effects[index].Trigger.Threshold;
            foreach (int proc in Enumerable.Range(1, procs))
                Data.Effects[index].Effect.Proc(owner, command);
            
            progress[index] = fullProgress % Data.Effects[index].Trigger.Threshold;
        }
    }
}