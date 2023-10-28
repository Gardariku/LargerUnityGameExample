using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Setup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using World.Data;
using World.Objects.Events;
using World.Objects.Interactions;

namespace World
{
    // TODO: Can't find a better to do this at the moment, has to do at least something
    public class WorldSerializer : MonoBehaviour
    {
        [SerializeField] private WorldState _currentState;
        [Space] 
        private SceneLoader _sceneLoader;
        private WorldPayload _payload;
        [SerializeField] private WorldMap _worldMap;
        [Space]
        [SerializeField] private AssetReference[] _enemies;
        [SerializeField] private AssetReference[] _backgrounds;
        [SerializeField] private InteractableObjectData[] _intObjectsData;
        [SerializeField] private string[] _worldEventNames;
        [SerializeField] private string[] _interactionTypeNames;

        [Inject]
        public void Init(SceneLoader sceneLoader, WorldPayload worldPayload)
        {
            _sceneLoader = sceneLoader;
            _payload = worldPayload;
        }
        
        public Dictionary<object, int> EnemyIdByWeakRef { get; private set; } = new();
        public Dictionary<int, AssetReference> EnemyWeakRefById { get; private set; } = new();
        public Dictionary<object, int> BackgroundIdByWeakRef { get; private set; } = new();
        public Dictionary<int, AssetReference> BackgroundWeakRefById { get; private set; } = new();
        public Dictionary<InteractableObjectData, int> IObjectIdByData { get; private set; } = new();
        public Dictionary<int, InteractableObjectData> IObjectDataById { get; private set; } = new();
        public Dictionary<Type, int> WEventIdByType { get; private set; } = new();
        public Dictionary<int, Type> WEventTypeById { get; private set; } = new();

        public Dictionary<Type, Func<WorldEventState, WorldSerializer, IWorldEvent>> WEventConstructorByType { get; } = new()
        {
            { typeof(StartBattleWorldEvent), (s, serializer) =>
            {
                var state = (StartBattleWorldEvent.StartBattleState) s;
                var enemies = new List<AssetReference>();
                foreach (var id in state.Enemies)
                    enemies.Add(serializer.EnemyWeakRefById[id]);
                return new StartBattleWorldEvent(enemies, serializer.BackgroundWeakRefById[state.BG]);
            } },
            {typeof(ReadNoteWorldEvent), (s, serializer) =>
            {
                var state = (ReadNoteWorldEvent.ReadNoteState) s;
                return new ReadNoteWorldEvent(state.Header, state.Message);
            }},
        };
        public Dictionary<Type, int> InteractionIdByType { get; private set; } = new();
        public Dictionary<int, Type> InteractionTypeById { get; private set; } = new();

        private void Awake()
        {
            for (int i = 0; i < _enemies.Length; i++)
                EnemyWeakRefById.Add(i, _enemies[i]);
            EnemyIdByWeakRef = EnemyWeakRefById.ToDictionary(x => x.Value.RuntimeKey, x => x.Key);
            
            for (int i = 0; i < _backgrounds.Length; i++)
                BackgroundWeakRefById.Add(i, _backgrounds[i]);
            BackgroundIdByWeakRef = BackgroundWeakRefById.ToDictionary(x => x.Value.RuntimeKey, x => x.Key);
            
            for (int i = 0; i < _intObjectsData.Length; i++)
                IObjectIdByData.Add(_intObjectsData[i], i);
            IObjectDataById = IObjectIdByData.ToDictionary(x => x.Value, x => x.Key);
            
            for (int i = 0; i < _worldEventNames.Length; i++)
                WEventIdByType.Add(Type.GetType($"World.Objects.Events.{_worldEventNames[i]}"), i);
            WEventTypeById = WEventIdByType.ToDictionary(x => x.Value, x => x.Key);

            
            for (int i = 0; i < _interactionTypeNames.Length; i++)
                InteractionIdByType.Add(Type.GetType($"World.Objects.Interactions.{_interactionTypeNames[i]}"), i);
            InteractionTypeById = InteractionIdByType.ToDictionary(x => x.Value, x => x.Key);
        }

        private void Start()
        {
            _sceneLoader.LoadingScene += type =>
            {
                if (type != SceneType.World) Serialize();
            };
        }

        public void Serialize()
        {
            using MemoryStream stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, new WorldState(_worldMap, this));
            _payload.WorldState = Convert.ToBase64String(stream.ToArray());
        }

        public WorldState Deserialize(string state)
        {
            using MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
            _currentState = new BinaryFormatter().Deserialize(stream) as WorldState;
            return _currentState;
        }

        [Serializable]
        public class WorldState
        {
            public Vector2IntS MapSize;
            public Vector2IntS PlayerPosition;
            public Vector2IntS[] Obstacles;
            public Interaction.InteractableObjectState[] InteractableObjects;

            public WorldState(WorldMap map, WorldSerializer serializer)
            {
                MapSize = new Vector2IntS(map.Height, map.Width);
                PlayerPosition = new Vector2IntS(map.PlayerCharacter.Cell.GridPosition);
                
                InteractableObjects = new Interaction.InteractableObjectState[map.InteractableObjects.Count];
                for (int i = 0; i < map.InteractableObjects.Count; i++)
                    InteractableObjects[i] = new Interaction.InteractableObjectState(map.InteractableObjects[i], serializer);

                Obstacles = new Vector2IntS[map.Obstacles.Count];
                for (int i = 0; i < map.Obstacles.Count; i++)
                    Obstacles[i] = new Vector2IntS(map.Obstacles[i].Cell.GridPosition);
            }
        }
    }
    
    [Serializable]
    public struct Vector2IntS
    {
        public int x;
        public int y;

        public Vector2IntS(Vector2Int vec)
        {
            x = vec.x;
            y = vec.y;
        }
        
        public Vector2IntS(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public static implicit operator Vector2Int(Vector2IntS sv) => new (sv.x, sv.y);
        public static implicit operator Vector2IntS(Vector2Int v) => new (v.x, v.y);
    }
}