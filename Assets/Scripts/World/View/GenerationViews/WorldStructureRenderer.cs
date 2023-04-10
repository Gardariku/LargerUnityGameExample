using System.Collections.Generic;
using System.Linq;
using BrewedInk.WFC;
using UnityEngine;

namespace World.PCD.StructureWFC
{
    public class WorldStructureRenderer : MonoBehaviour
    {
        [field: SerializeField] public WorldGenerator WorldGenerator { get; private set; }
        /// <summary>
        /// Any configuration that contains sprites for previews. 
        /// </summary>
        public WorldStructureConfig config;

        /// <summary>
        /// The sprite to use for the bounds of the board.
        /// </summary>
        public SpriteRenderer board;

        public WorldStructureSlotRenderer slotPrefab;
        
        public List<WorldStructureSlotRendererOption> SlotOptionGameObjects;

        public Dictionary<Transform, WorldStructureSlotRendererOption> colliderToSlotOption;

        [Range(0, 1)] public float Gutter = 1;

        private GenerationSpace _space;

        private Dictionary<Slot, WorldStructureSlotRenderer> slotToRenderer = new ();

        // Start is called before the first frame update
        void Start()
        {
            WorldGenerator.GeneratedSkeleton += OnSkeletonGenerated;
            InitWFC();
        }

        public void RelaxOne()
        {
            _space.CollapseLowestEntropySlot().RunAsCoroutine(this, frameBudgetTime).OnProgress(HandleProgress);
        }

        public void SolveStructure()
        {
            _space.Collapse().RunAsCoroutine(this, frameBudgetTime).OnProgress(HandleProgress);
        }

        public void SolveSkeleton()
        {
            WorldGenerator.GenerateSkeleton();
        }

        public void RemoveModule(Slot slot, Module option)
        {
            _space.RemoveSlotOption(slot, option).RunAsCoroutine(this, frameBudgetTime).OnProgress(HandleProgress);
        }

        public void SelectionModule(Slot slot, Module option)
        {
            _space.CollapseSlot(slot, option).RunAsCoroutine(this, frameBudgetTime).OnProgress(HandleProgress);
        }

        public void Reset()
        {
            _space.Reset();
            config.FormPrerequisites(_space);
            InitGameObjects();
        }

        public void Check()
        {
            var errors = _space.Validate();
            Debug.Log("ERRORS: " + errors);
        }

        private void OnSkeletonGenerated()
        {
            for (int i = 0; i < WorldGenerator.Width; i++)
            {
                for (int j = 0; j < WorldGenerator.Height; j++)
                    if (WorldGenerator.WorldSkeleton[i, j] == 1)
                        _space.CollapseSlot(_space.GetSlot(new(i, j)), _space.AllModules.FindByDisplay("O"))
                            .RunAsCoroutine(this, frameBudgetTime).OnProgress(HandleProgress);
            }
        }

        [Range(0.001f, .2f)] public float frameBudgetTime = .1f;

        void HandleProgress(WFCProgress progress)
        {
            switch (progress)
            {
                case SlotModuleSelected selected:
                    slotToRenderer[selected.slot].SelectOption(selected.module);
                    break;
                case SlotModuleRemoved removal:
                    slotToRenderer[removal.slot].RemoveOption(removal.module);
                    break;
            }
        }


        public void InitWFC()
        {
            _space = config.Create();
            // foreach slot in the space, we need to create a prefab for it... 

            // also, we need to place and size them within the board bounds... 
            InitGameObjects();
        }

        public void InitGameObjects()
        {
            slotToRenderer.Clear();
            var allOptionGobs = new List<WorldStructureSlotRendererOption>();

            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var minY = int.MaxValue;

            foreach (var slot in _space.Slots)
            {
                minX = Mathf.Min(slot.Coordinate.x, minX);
                minY = Mathf.Min(slot.Coordinate.y, minY);
                maxX = Mathf.Max(slot.Coordinate.x, maxX);
                maxY = Mathf.Max(slot.Coordinate.y, maxY);
            }

            var width = (maxX - minX) + 1; // account for zero.
            var height = (maxY - minY) + 1;

            var widthScaleRatio = (1f / width) - (Gutter * (1f / width));
            var heightScaleRatio = (1f / height) - (Gutter * (1f / height));

            for (var i = 0; i < board.transform.childCount; i++)
            {
                Destroy(board.transform.GetChild(i).gameObject);
            }

            foreach (var slot in _space.Slots)
            {
                var slotInstance = Instantiate(slotPrefab, board.transform);

                slotToRenderer.Add(slot, slotInstance);
                slotInstance.transform.localScale = new Vector3(widthScaleRatio, heightScaleRatio, 1);

                slotInstance.name = $"slot {slot.Coordinate}";
                slotInstance.gridRenderer = this;
                var xRatio = (slot.Coordinate.x + .5f) / (float) width;
                var yRatio = (slot.Coordinate.y + .5f) / (float) height;
                var x = ((xRatio - .5f) * 1); //- realBounds.extents.x;// * bounds.size.x;
                var y = ((yRatio - .5f) * 1); // - realBounds.extents.y;
                slotInstance.transform.localPosition = new Vector3(x, y, 0);

                var createdSlotOptions = slotInstance.OnCreated(config, _space, slot);
                allOptionGobs.AddRange(createdSlotOptions);
            }

            SlotOptionGameObjects = allOptionGobs;
            colliderToSlotOption = allOptionGobs.ToDictionary(g => g.transform);
        }

        public void Generate()
        {

        }
    }
}