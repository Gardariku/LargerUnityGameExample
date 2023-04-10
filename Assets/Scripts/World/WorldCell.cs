using System;
using UnityEngine;

namespace World
{
    public class WorldCell : MonoBehaviour
    {
        [field: SerializeField] public Vector2Int GridPosition { get; set; }
        [field: SerializeField] public SurfaceType Type { get; private set; }
        [SerializeField] private bool _isBlocked;
        [SerializeField] private bool _isFree;

        public bool IsPassable => !_isBlocked && _isFree;

        public void Visit()
        {
            if (_isFree)
                _isFree = false;
            else
                throw new Exception($"Trying to visit busy cell at {GridPosition}");
        }
        
        public void Leave()
        {
            if (!_isFree)
                _isFree = true;
            else
                throw new Exception($"Trying to leave free cell at {GridPosition}");
        }
    }

    public enum SurfaceType
    {
        Void,
        Water,
        Grass,
        Desert,
        Rocks
    }
}