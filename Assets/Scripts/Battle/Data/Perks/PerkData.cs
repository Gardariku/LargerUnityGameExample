using System;
using Battle.Controller;
using UnityEngine;

namespace Battle.Data.Perks
{
    [CreateAssetMenu(fileName = "New Perk", menuName = "Build/Perks/Perk")]
    public class PerkData : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        [field: Space][field: SerializeField]
        public PerkEffect[] Effects { get; private set; }

        public void Activate(Perk perk, Character character)
        {
            foreach (var effect in Effects)
                effect.Trigger.Subscribe(perk, character);
        }

        public int GetEffectID(PerkTriggerData trigger)
        {
            for (int i = 0; i < Effects.Length; i++)
            {
                if (Effects[i].Trigger == trigger)
                    return i;
            }

            return -1;
        }

        [Serializable]
        public class PerkEffect
        {
            [field: SerializeReference, SubclassSelector]
            public PerkEffectData Effect { get; private set; }
            [field: SerializeReference, SubclassSelector]
            public PerkTriggerData Trigger { get; private set; }
        }
    }
}
