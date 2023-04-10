using System;
using UnityEngine;
using World.Objects;
using World.View;

namespace World.Characters
{
    public class Player : MonoBehaviour
    {
        [field: SerializeField] public WorldObject WorldObject { get; private set; }
        [field: SerializeField] public CharacterMovement Movement { get; private set; }
        [field: SerializeField] public PlayerInteractions Interaction { get; private set; }
        [field: SerializeField] public CharacterWorldView Visual { get; private set; }
    }
}