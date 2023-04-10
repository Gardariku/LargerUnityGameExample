using System;
using UnityEngine;

namespace World.Objects
{
    public class WorldObject : MonoBehaviour
    {
        [field: SerializeField] public WorldCell Cell { get; set; }
        [field: SerializeField] public Vector2Int Size { get; set; }
        [field: SerializeField] public bool Block { get; set; }

        public static WorldObject None = new ();

        public void Init(WorldObjectState state)
        {
            Size = state.Size;
            Block = state.Block;
        }
    }

    public enum WorldObjectType
    {
        Tile,
        Obstacle,
        Interactable,
        Character
    }

    [Serializable]
    public class WorldObjectState
    {
        public Vector2IntS Position;
        public Vector2IntS Size;
        public bool Block;

        public WorldObjectState(WorldObject worldObject)
        {
            Position = new Vector2IntS(worldObject.Cell.GridPosition);
            Size = new Vector2IntS(worldObject.Size);
            Block = worldObject.Block;
        }
    }
}