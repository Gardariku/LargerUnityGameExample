using System.Collections.Generic;
using System.Linq;
using BrewedInk.WFC;
using UnityEngine;

namespace World.PCD.StructureWFC
{
    [CreateAssetMenu(fileName="New World Structure Config", menuName="BrewedInk WFC/World Structure Config")]
    public class WorldStructureConfig : WCFConfigObject<WorldStructureModuleObject, WorldStructureModule>
    {
        public int Width = 16;
        public int Height = 16;
        protected override GenerationSpace CreateSpace()
        {
            WorldStructureWFC solver = new WorldStructureWFC();
            GenerationSpace space = GenerationSpace.From2DGrid(Width, Height, GetModules().ProduceConstraints(solver),
                useSeed ? seed : default, PrepareSpace);
            
            FormModuleSubsets(space);
            FormPrerequisites(space);
            
            return space;
        }

        // For now this just regulates possible module options at border slots
        public void FormPrerequisites(GenerationSpace space)
        {
            foreach (var slot in space.Slots)
            {
                if (slot.Coordinate.x == 0 || slot.Coordinate.x == Width - 1
                    || slot.Coordinate.y == 0 || slot.Coordinate.y == Height - 1)
                    space.GetSlotOptions(slot).RemoveWhere(x => !BorderlineTiles.Contains(x));
            }
        }

        private void PrepareSpace(List<Slot> slots, List<SlotEdge> edges)
        {
            var slotDict = slots.ToDictionary(s => s.Coordinate);
            foreach (var slot in slots)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        slotDict.TryGetValue(new(slot.Coordinate.x + i, slot.Coordinate.y + j, 0), out var neighbor);
                        if (neighbor != null) edges.Add(new SlotEdge {Source = slot, Target = neighbor});
                    }
                }
            }
        }

        private void FormModuleSubsets(GenerationSpace space)
        {
            var objectDisplays = new List<string> {"T", "S", "L", "G", "E"}; 
            var passableDisplays = new List<string> {".", "R"}; 
            var borderlineDisplays = new List<string> {".", "R", "O", "B"}; 
            InteractableObjects.Clear();
            PassableTiles.Clear();
            BorderlineTiles.Clear();
            foreach (var display in objectDisplays)
                InteractableObjects.Add(space.AllModules.FindByDisplay(display));
            foreach (var display in passableDisplays)
                PassableTiles.Add(space.AllModules.FindByDisplay(display));
            foreach (var display in borderlineDisplays)
                BorderlineTiles.Add(space.AllModules.FindByDisplay(display));
        }
        
        public override bool TryGetSprite(Module module, out Sprite sprite)
        {
            if (TryGetObject(module, out var obj))
            {
                sprite = obj.PreviewSprite;
                return true;
            }

            sprite = null;
            return false;
        }

        public static List<Module> InteractableObjects { get; } = new ();
        public static List<Module> PassableTiles { get; } = new ();
        public static List<Module> BorderlineTiles { get; } = new ();
    }
    
    [System.Serializable]
    public class WorldStructureModuleObject : ModuleObject<WorldStructureModule>
    {
        public Sprite PreviewSprite;
    }
}