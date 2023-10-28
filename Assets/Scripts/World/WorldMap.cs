using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;
using World.Objects;
using World.Objects.Interactions;

namespace World
{
    // I find controller-view-data separation excessive and inconvenient for describing world map logic,
    // so more component-oriented approach will be used here instead
    public class WorldMap : MonoBehaviour
    {
        private WorldObjectFactory _factory;
        private Tilemap _tilemap;
        private WorldGenerator _worldGenerator;
        [field: SerializeField] public List<Interaction> InteractableObjects { get; private set; }
        [field: SerializeField] public List<WorldObject> Obstacles { get; private set; }
        [field: SerializeField] public WorldObject PlayerCharacter { get; private set; }
        [Space] 
        [SerializeField] private Transform _cellsTransform;
        [SerializeField] private Transform _obstaclesTransform;
        [SerializeField] private Transform _objectsTransform;
        
        public WorldCell[,] Cells { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public event Action CreatedWorld;

        [Inject]
        public void Init(WorldObjectFactory objectFactory, WorldGenerator worldGenerator, Tilemap tilemap)
        {
            _tilemap = tilemap;
            _factory = objectFactory;
            _worldGenerator = worldGenerator;
        }

        public void GenerateNewMap()
        {
            _worldGenerator.GenerateWorld();
            Height = _worldGenerator.Height;
            Width = _worldGenerator.Width;
            
            SetupWorldMap();
            PlaceBorders();
            CreatedWorld?.Invoke();
        }

        public void LoadMap(WorldSerializer.WorldState state)
        {
            Height = state.MapSize.x;
            Width = state.MapSize.y;

            RestoreWorldMap(state);
            PlaceBorders();
            CreatedWorld?.Invoke();
        }


        // TODO: Clean this later
        private void PlaceBorders()
        {
            for (int x = -1; x <= Width; x++)
            {
                for (int y = -1; y <= Height; y++)
                {
                    if (y == -1)
                    {
                        if (x == -1) PlaceTile(_factory.CreateBorderTile(0),new (x, y), _tilemap.transform);
                        else if (x == Width) PlaceTile(_factory.CreateBorderTile(2),new (x, y), _tilemap.transform);
                        else PlaceTile(_factory.CreateBorderTile(1),new (x, y), _tilemap.transform);
                    }
                    else if (y < Height)
                    {
                        if (x == Width) PlaceTile(_factory.CreateBorderTile(3),new (x, y), _tilemap.transform);
                        if (x == -1) PlaceTile(_factory.CreateBorderTile(7),new (x, y), _tilemap.transform);
                    }
                    else
                    {
                        if (x == Width) PlaceTile(_factory.CreateBorderTile(4),new (x, y), _tilemap.transform);
                        else if (x == -1) PlaceTile(_factory.CreateBorderTile(6),new (x, y), _tilemap.transform);
                        else PlaceTile(_factory.CreateBorderTile(5),new (x, y), _tilemap.transform);
                    }
                }
            }
        }

        // TODO: remove boilerplate
        private void SetupWorldMap()
        {
            Cells = new WorldCell[Height, Width];
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    Cells[x, y] = _factory.CreateTile(_worldGenerator.SurfaceCells[x, y]).GetComponent<WorldCell>();
                    Cells[x, y].GridPosition = new(x, y);
                    PlaceTile(Cells[x,y].gameObject, new (x, y), _cellsTransform);
                }
            }

            InteractableObjects = new List<Interaction>();
            foreach (var entry in _worldGenerator.InteractableObjects)
            {
                var io = _factory.CreateInteractableObject(entry.Item2.Type);
                io.WorldObject.Cell = Cells[entry.Item1.x, entry.Item1.y];
                BlockObjectTiles(io.WorldObject);
                PlaceTile(io.gameObject, entry.Item1, _objectsTransform.transform);
                InteractableObjects.Add(io);
            }
            
            Obstacles = new List<WorldObject>();
            foreach (var pos in _worldGenerator.Obstacles)
            {
                var obstacleObject = _factory.CreateObstacle().GetComponent<WorldObject>();
                obstacleObject.Cell = Cells[pos.x, pos.y];
                BlockObjectTiles(obstacleObject);
                PlaceTile(obstacleObject.gameObject, pos, _obstaclesTransform.transform);
                Obstacles.Add(obstacleObject);
            }

            var startPos = _worldGenerator.PlayerStart;
            PlayerCharacter.Cell = Cells[startPos.x, startPos.y];
            BlockObjectTiles(PlayerCharacter);
            PlayerCharacter.transform.parent = _tilemap.transform.parent;
            PlayerCharacter.transform.position = PlayerCharacter.Cell.transform.position;
        }
        
        private void RestoreWorldMap(WorldSerializer.WorldState state)
        {
            Cells = new WorldCell[Height, Width];
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    // TODO: Serialize surface type in WorldState
                    Cells[x, y] = _factory.CreateTile(SurfaceType.Grass).GetComponent<WorldCell>();
                    Cells[x, y].GridPosition = new(x, y);
                    PlaceTile(Cells[x,y].gameObject, new (x, y), _cellsTransform);
                }
            }
            
            Obstacles = new List<WorldObject>();
            foreach (var pos in state.Obstacles)
            {
                var obstacleObject = _factory.CreateObstacle().GetComponent<WorldObject>();
                obstacleObject.Cell = Cells[pos.x, pos.y];
                BlockObjectTiles(obstacleObject);
                PlaceTile(obstacleObject.gameObject, pos, _obstaclesTransform.transform);
                Obstacles.Add(obstacleObject);
            }
            
            InteractableObjects = new List<Interaction>();
            foreach (var entry in state.InteractableObjects)
            {
                var io = _factory.RecoverInteractableObject(entry);
                io.WorldObject.Cell = Cells[entry.WObject.Position.x, entry.WObject.Position.y];
                BlockObjectTiles(io.WorldObject);
                PlaceTile(io.gameObject, io.WorldObject.Cell.GridPosition, _objectsTransform.transform);
                InteractableObjects.Add(io);
            }
            
            var startPos = state.PlayerPosition;
            PlayerCharacter.Cell = Cells[startPos.x, startPos.y];
            BlockObjectTiles(PlayerCharacter);
            PlayerCharacter.transform.parent = _tilemap.transform.parent;
            PlayerCharacter.transform.position = PlayerCharacter.Cell.transform.position;
        }

        private void PlaceTile(GameObject obj, Vector2Int pos, Transform target)
        {
            obj.transform.parent = target;
            obj.transform.position = _tilemap.GetCellCenterWorld(new(pos.x, pos.y));
        }

        private void BlockObjectTiles(WorldObject obj)
        {
            for (int i = 0; i < obj.Size.x; i++)
            {
                for (int j = 0; j < obj.Size.y; j++)
                    Cells[obj.Cell.GridPosition.x + i, obj.Cell.GridPosition.y + j].Visit();
            }
        }

        [Serializable]
        private class WorldContent
        {
            [field: SerializeField] public int Test { get; private set; }
        }
    }
}
