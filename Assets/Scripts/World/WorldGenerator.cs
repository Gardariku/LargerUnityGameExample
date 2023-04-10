using System;
using System.Collections.Generic;
using BrewedInk.WFC;
using UnityEngine;
using World.Data;
using World.PCD.SkeletonCA;
using World.PCD.StructureWFC;
using Random = UnityEngine.Random;

namespace World
{
    public class WorldGenerator : MonoBehaviour
    {
        [field: SerializeField] public int Width { get; private set; } = 16;
        [field: SerializeField] public int Height { get; private set; } = 16;
        [field: SerializeField] public int ObstaclePercent { get; private set; } = 35;
        [field: SerializeField] public WorldStructureConfig Config { get; private set; }
        [field: Space]
        [field: SerializeField] public int WorldValue { get; private set; } = 100;
        [field: SerializeField] public InteractableObjectData[] PossibleObjects { get; private set; }
        private ObstaclesGenerator _skeletonGenerator;
        public byte[,] WorldSkeleton { get; private set; }
        public GenerationSpace Structure { get; private set; }
        public SurfaceType[,] SurfaceCells { get; private set; }
        public List<Vector2Int> Obstacles { get; } = new ();
        public List<(Vector2Int, InteractableObjectData)> InteractableObjects { get; } = new ();
        public Vector2Int PlayerStart { get; private set; } 

        #region Events
        public event Action GeneratedSkeleton;
        public event Action GeneratedStructure;
        public event Action GeneratedAppearence;
        public event Action GeneratedWorld;
        #endregion

        public void GenerateWorld()
        {
            GenerateSkeleton();
            
            PrepareStructureGeneration();
            GenerateStructure();
            
            GenerateContent();
            GeneratedWorld?.Invoke();
        }

        private void PrepareSkeletonGeneration()
        {
            Width = Config.Width;
            Height = Config.Height;
            WorldSkeleton = new byte[Width, Height];
            _skeletonGenerator = new ObstaclesGenerator(WorldSkeleton, Width, Height);
            
            // Установить супер-черные клетки
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++) {
                    int rand = Random.Range(0, 100) < ObstaclePercent ? 1 : 0;
                    WorldSkeleton[x, y] = (byte)rand;
                }
            }

            // Установить супер-белые клетки
            PlayerStart = new Vector2Int(0, Height / 2);
            WorldSkeleton[PlayerStart.x, PlayerStart.y] = 2;
        }

        public void GenerateSkeleton()
        {
            PrepareSkeletonGeneration();
            WorldSkeleton = _skeletonGenerator.GetResult(3);
            GeneratedSkeleton?.Invoke();
        }

        private void PrepareStructureGeneration()
        {
            Structure = Config.Create();
            for (int x = 0; x < WorldSkeleton.GetLength(0); x++)
            {
                for (int y = 0; y < WorldSkeleton.GetLength(1); y++)
                {
                    if (WorldSkeleton[x, y] == 1)
                        Structure.CollapseSlot(Structure.GetSlot(new(x, y)),
                            Structure.AllModules.FindByDisplay("O")).RunAsImmediate();
                }
            }
        }

        private void GenerateStructure()
        {
            Structure.Collapse().RunAsImmediate();
            
            FillSurfaces();
            GeneratedStructure?.Invoke();
        }

        // TODO: Come up with sub-algorithm for placing different surfaces (these also need to be implemented)
        private void FillSurfaces()
        {
            SurfaceCells = new SurfaceType[Width, Height];
            foreach (var slot in Structure.Slots)
            {
                if (!Structure.TryGetOnlyOption(slot, out var module))
                    throw new Exception($"Structure slot at {slot.Coordinate} doesn't have a single option");
                //if (Equals(module, Structure.AllModules.FindByDisplay(".")))
                    SurfaceCells[slot.Coordinate.x, slot.Coordinate.y] = SurfaceType.Grass;
            }
        }

        private void GenerateContent()
        {
            var smallObjects = new List<Vector2Int>();
            var largeObjects = new List<Vector2Int>();
            var guardedObjects = new List<Vector2Int>();
            
            foreach (var slot in Structure.Slots)
            {
                if (!Structure.TryGetOnlyOption(slot, out var module))
                    throw new Exception($"Structure slot at {slot.Coordinate} doesn't have a single option");
                
                // lame, but will leave this for now, it's all subject to change anyway
                if (Equals(module, Structure.AllModules.FindByDisplay("O")))
                    Obstacles.Add(new Vector2Int(slot.Coordinate.x, slot.Coordinate.y));
                if (Equals(module, Structure.AllModules.FindByDisplay("S")))
                    smallObjects.Add(new Vector2Int(slot.Coordinate.x, slot.Coordinate.y));
                if (Equals(module, Structure.AllModules.FindByDisplay("L")))
                    largeObjects.Add(new Vector2Int(slot.Coordinate.x, slot.Coordinate.y));
                if (Equals(module, Structure.AllModules.FindByDisplay("G")))
                    guardedObjects.Add(new Vector2Int(slot.Coordinate.x, slot.Coordinate.y));
            }

            DistributeValue(smallObjects, largeObjects, guardedObjects);
        }

        // TODO: Come up with some algorithm (like in the backpack problem) to distribute summary map value
        // across allocated object types, while keeping high diversity
        private void DistributeValue(List<Vector2Int> small, List<Vector2Int> large, List<Vector2Int> guarded)
        {
            foreach (var position in small)
                InteractableObjects.Add(new (position, PossibleObjects[0]));
            foreach (var position in large)
                InteractableObjects.Add(new (position, PossibleObjects[1]));
            foreach (var position in guarded)
                InteractableObjects.Add(new (position, PossibleObjects[2]));
        }
    }
}
