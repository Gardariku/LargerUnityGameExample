using System;
using System.Collections.Generic;
using BrewedInk.WFC;
using UnityEngine;

namespace World.PCD.StructureWFC
{
    [Serializable]
    public class WorldStructureModule : Module
    {
        public StructureTile Type;

        public static StructureTile Objects = StructureTile.Treasure & StructureTile.GuardedObject &
                                              StructureTile.SmallObject & StructureTile.LargeObject;
    }

    [Flags]
    public enum StructureTile
    {
        Border,
        Free,
        Road,
        Obstacle,
        Water,
        Block,
        Enemy,
        Treasure,
        SmallObject,
        LargeObject,
        GuardedObject
    }
    
    public class Window2Constraint : ModuleConstraint
    {
        // TODO: Change window source to higher left corner
        public static IEnumerable<Slot> GetNeighbors(Vector3Int slotCoordinate, GenerationSpace space)
        {
            for (var x = slotCoordinate.x; x < slotCoordinate.x + 1; x++)
            {
                for (var y = slotCoordinate.y; y < slotCoordinate.y + 1; y++)
                {
                    var coord = new Vector3Int(x, y, 0);
                    if (coord == slotCoordinate) continue;
                    yield return space.GetSlot(coord);
                }
            }
        }
        public override bool ShouldRemoveModule(SlotEdge edge, GenerationSpace space, Module module, ModuleSet modulesToRemove)
        {
            throw new NotImplementedException();
        }
    }
    
    public class Window3Constraint : ModuleConstraint
    {
        public static IEnumerable<Slot> GetNeighbors(Vector3Int slotCoordinate, GenerationSpace space)
        {
            for (var x = slotCoordinate.x; x < slotCoordinate.x + 2; x++)
            {
                for (var y = slotCoordinate.y; y < slotCoordinate.y + 2; y++)
                {
                    var coord = new Vector3Int(x, y, 0);
                    if (coord == slotCoordinate) continue;
                    yield return space.GetSlot(coord);
                }
            }
        }
        
        public override bool ShouldRemoveModule(SlotEdge edge, GenerationSpace space, Module module, ModuleSet modulesToRemove)
        {
            var source = edge.Source;
            var target = edge.Target;
            var sourceOptions = space.GetSlotOptions(source);
            var targetOptions = space.GetSlotOptions(target);

            // TODO: Think about how to cache modules
            // Place Blocks in a 2x2 square near LargeObject tile
            if (source.Coordinate.x >= target.Coordinate.x && source.Coordinate.y >= target.Coordinate.y 
                && targetOptions.Count == 1 && targetOptions.Contains(space.AllModules.FindByDisplay("L")))
            {
                space.CollapseSlot(source, space.AllModules.FindByDisplay("B"));
            }
            // And if tiles are already determined, then LargeObject can't be placed below them
            if (source.Coordinate.x <= target.Coordinate.x && source.Coordinate.y <= target.Coordinate.y &&
                !targetOptions.Contains(space.AllModules.FindByDisplay("B")))
            {
                modulesToRemove.Add(space.AllModules.FindByDisplay("L"));
            }

            // Tile below InteractableObject should be free to pass
            if ((source.Coordinate.x == target.Coordinate.x || source.Coordinate.y < target.Coordinate.y ) &&
                targetOptions.IsSubsetOf(WorldStructureConfig.InteractableObjects))
            {
                modulesToRemove.Add(space.AllModules.FindByDisplay("O"));
            }
            // InteractableObjects can't be placed above obstacle
            if (source.Coordinate.x == target.Coordinate.x && source.Coordinate.y > target.Coordinate.y
                && targetOptions.Count == 1 && targetOptions.Contains(space.AllModules.FindByDisplay("O")))
            {
                foreach (var sourceOption in sourceOptions)
                {
                    if (WorldStructureConfig.InteractableObjects.Contains(sourceOption)) 
                        modulesToRemove.Add(sourceOption);
                }
            }
            
            // InteractableObjects shouldn't be close to each other
            if (targetOptions.IsSubsetOf(WorldStructureConfig.InteractableObjects))
            {
                foreach (var sourceOption in sourceOptions)
                {
                    if (WorldStructureConfig.InteractableObjects.Contains(sourceOption)) 
                        modulesToRemove.Add(sourceOption);
                }
            }

            return false;
        }
    }

    public class WorldStructureWFC : ConstraintGenerator<WorldStructureModule>
    {
        public override WorldStructureModule Copy(WorldStructureModule module, List<ModuleConstraint> constraints)
        {
            return new WorldStructureModule
            {
                Constraints = constraints,
                Type = module.Type,
                Display = module.Display,
                Weight = module.Weight
            };
        }
        
        public override List<ModuleConstraint> CreateConstraints(WorldStructureModule source)
        {
            var constraints = new List<ModuleConstraint>();
            constraints.Add(new Window3Constraint());
            return constraints;
        }

        public override List<ModuleConstraint> CreateConstraints(WorldStructureModule source, WorldStructureModule target)
        {
            return null;
        }
    }
}