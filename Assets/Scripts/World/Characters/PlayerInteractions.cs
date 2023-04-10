using System.Collections.Generic;
using UnityEngine;
using World.Objects.Interactions;

namespace World.Characters
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] private List<Interaction> _availableInteractions = new ();
        [SerializeField] private Interaction _currentInteraction;

        public void AddInteraction(Interaction interaction)
        {
            _availableInteractions.Add(interaction);
        }
        
        public void RemoveInteraction(Interaction interaction)
        {
            _availableInteractions.Remove(interaction);
        }
        
        public bool TryInteract()
        {
            foreach (var interaction in _availableInteractions)
            {
                _currentInteraction = interaction;
                interaction.Interact();
                return true;
            }

            return false;
        }

        public void FinishInteraction(int result = -1)
        {
            _currentInteraction.AcceptResult(result);
            _currentInteraction = null;
        }
    }
}