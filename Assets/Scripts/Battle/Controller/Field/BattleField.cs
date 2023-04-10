using System.Collections.Generic;
using System.Linq;
using Battle.Controller.Commands;
using Battle.Model;
using UnityEngine;

namespace Battle.Controller.Field
{
    public class BattleField
    {
        public Vector2Int Size => new (Cells.GetLength(0), Cells.GetLength(1));
        public IFieldObject[,] Cells { get; }
        public List<Vector2Int> Obstacles { get; } = new ();

        private static Path _pathFinder = new ();
        private DepthFirstSearch _search = new (StraightMoveCost, DiagonalMoveCost);
        private const float DiagonalMoveCost = 1.5f;
        private const float StraightMoveCost = 1f;

        public BattleField(Vector2Int size, IEnumerable<IFieldObject> characters, BattleController controller)
        {
            Cells = new IFieldObject[size.x,size.y];
            // TODO: Finish method
            // PlaceObstacles();
            // AdjustInitialCharactersPositions(); (take obstacles into account)
            foreach (var character in characters)
            {
                Cells[character.Position.x, character.Position.y] = character;
            }
            
            SubscribeOnEvents(controller);
        }

        private void SubscribeOnEvents(BattleController controller)
        {
            controller.CharacterEvents.CharacterStepFinished += OnCharacterStepFinished;
        }

        private void OnCharacterStepFinished(StepCommand step)
        {
            Cells[step.Start.x, step.Start.y] = null;
            Cells[step.Finish.x, step.Finish.y] = step.Actor;
        }

        public Vector2Int[] FindPath(Vector2Int start, Vector2Int finish)
        {
            _pathFinder.Calculate(start, finish, Obstacles, out var path);
            return path.ToArray();
        }

        public float CalculatePathLength(IEnumerable<Vector2Int> path)
        {
            float sum = 0f;
            bool first = true;
            Vector2Int prev = Vector2Int.zero;
            foreach (var cell in path)
            {
                if (first)
                {
                    prev = cell;
                    first = false;
                    continue;
                }

                if (prev.x != cell.x && prev.y != cell.y)
                    sum += DiagonalMoveCost;
                else
                    sum += StraightMoveCost;
            }

            return sum;
        }

        public int CalculateDistance(IFieldObject object1, IFieldObject object2)
        {
            int x = object1.Position.x - object2.Position.x;
            int y = object1.Position.y - object2.Position.y;
            return Mathf.CeilToInt(Mathf.Sqrt(x * x - y * y));
        }

        public bool[,] GetFreeCellsMap()
        {
            var map = new bool[Size.x, Size.y];
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                    map[i, j] = Cells[i, j] != null;
            }
            return map;
        }

        public List<Vector2Int> GetPassableCells(IFieldObject actor)
        {
            return _search.Calculate(GetFreeCellsMap(), actor.Position, actor.Speed, actor.Size).ToList();
        }
        public void GetCharactersCells(out List<Vector2Int> allies, out List<Vector2Int> enemies)
        {
            allies = new List<Vector2Int>();
            enemies = new List<Vector2Int>();
            foreach (var cell in Cells)
            {
                if (cell is not Character character) continue;
                if (character.Team == Team.Player)
                    allies.Add(cell.Position);
                else 
                    enemies.Add(cell.Position);
            }
        }
    }
}