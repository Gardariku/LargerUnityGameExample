using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle.Controller.Field
{
    public class DepthFirstSearch
    {
        private float _straightCost;
        private float _diagonalCost;

        private HashSet<Vector2Int> _passable;
        private float _maxLength;
        private Vector2Int _startPosition;
        private Vector2Int _objectSize;

        public DepthFirstSearch(float straightCost, float diagonalCost)
        {
            _straightCost = straightCost;
            _diagonalCost = diagonalCost;
        }
        
        public IEnumerable<Vector2Int> Calculate(in bool[,] map, Vector2Int position, float length, Vector2Int size)
        {
            _maxLength = length;
            _startPosition = position;
            _objectSize = size;
            _passable = new HashSet<Vector2Int>();
            
            MoveNext(in map, position, 0f);

            return _passable;
        }

        private void MoveNext(in bool[,] map, Vector2Int position, float sum)
        {
            for (int i = Mathf.Max(0, position.x - 1); i < Mathf.Min(map.GetLength(0), position.x + 2); i++)
            {
                for (int j = Mathf.Max(0, position.y - 1); j < Mathf.Min(map.GetLength(1), position.y + 2); j++)
                {
                    var currentSum = sum;
                    var newPos = new Vector2Int(i, j);
                    if (i == position.x && j == position.y) continue;
                    if (!IsMovePossible(map, position, newPos)) continue;
                    
                    if (i != position.x && j != position.y) currentSum += _diagonalCost;
                    else currentSum += _straightCost;
                    if (currentSum > _maxLength + 0.05f) continue;
                    
                    _passable.Add(newPos);
                    MoveNext(in map, newPos, currentSum);
                }
            }
        }

        // This method implies that distance between start and finish is 1 cell, but i don't wanna check it here
        private bool IsMovePossible(in bool[,] map, Vector2Int start, Vector2Int finish)
        {
            for (int i = 0; i < _objectSize.x; i++)
            {
                for (int j = 0; j < _objectSize.y; j++)
                {
                    // Check if all cells which should be occupied by object after move exist and are free
                    // First 2 checks can be done once for the most right-up cell
                    var cell = finish + new Vector2Int(i, j);
                    if (cell.x >= map.GetLength(0) || cell.y >= map.GetLength(1) 
                        || cell.x < 0 || cell.y < 0 || map[cell.x, cell.y])
                        return false;
                    // Then you should probably make additional checks for near cells in case of diagonal movement,
                    // but we will leave this as a feature for now
                }
            }
            return true;
        }
    }
}