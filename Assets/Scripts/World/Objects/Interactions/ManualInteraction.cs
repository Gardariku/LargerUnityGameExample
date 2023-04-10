using System;
using UnityEngine;
using World.Characters;

namespace World.Objects.Interactions
{
    public class ManualInteraction : Interaction
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<PlayerInteractions>(out var player))
            {
                player.AddInteraction(this);
            }
        }
        
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.TryGetComponent<PlayerInteractions>(out var player))
            {
                player.RemoveInteraction(this);
            }
        }
    }
}