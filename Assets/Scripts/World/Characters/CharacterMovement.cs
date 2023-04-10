using System;
using UnityEngine;
using World.Objects;
using Zenject;

namespace World.Characters
{
    [RequireComponent(typeof(WorldObject))]
    public class CharacterMovement : MonoBehaviour
    {
        private WorldMap _worldMap;
        [SerializeField] private WorldObject _character;

        [Inject]
        public void Init(WorldMap worldMap)
        {
            _worldMap = worldMap;
        }

        public event Action<Vector2Int> Moved;

        public bool TryMove(Vector2Int direction)
        {
            var target = _character.Cell.GridPosition + direction;
            if (target.x < 0 || target.x >= _worldMap.Width || target.y < 0 || target.y >= _worldMap.Height)
                return false;
            if (!_worldMap.Cells[target.x, target.y].IsPassable)
                return false;

            _character.Cell.Leave();
            _character.Cell = _worldMap.Cells[target.x, target.y];
            _character.Cell.Visit();
            
            Moved?.Invoke(direction);
            return true;
        }
    }
}