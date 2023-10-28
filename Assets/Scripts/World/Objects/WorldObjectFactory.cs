using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using World.Data;
using World.Objects.Events;
using World.Objects.Interactions;
using Random = UnityEngine.Random;

namespace World.Objects
{
    public class WorldObjectFactory : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultWorldObject;
        [SerializeField] private TypePrefab<InteractableObjectType>[] _interactableObjects;
        [SerializeField] private TypePrefab<SurfaceType>[] _tiles;
        [SerializeField] private GameObject[] _obstacles;
        [SerializeField] private GameObject[] _borders;
        private WorldSerializer _serializer;
        private IObjectResolver _container;

        [Inject]
        public void Init(IObjectResolver container, WorldSerializer serializer)
        {
            _container = container;
            _serializer = serializer;
        }

        public Interaction CreateInteractableObject(InteractableObjectType subtype)
        {
            Interaction interaction = null;
            foreach (var obj in _interactableObjects)
            {
                if (obj == subtype)
                    interaction = _container.Instantiate(obj.Prefab).GetComponent<Interaction>();
            }

            if (interaction == null)
                throw new ArgumentException($"Cannot find appropriate prefab for {subtype} InteractableObject");
            
            return interaction;
        }

        public Interaction RecoverInteractableObject(Interaction.InteractableObjectState state)
        {
            Type interactionType = _serializer.InteractionTypeById[state.InteractionType];
            var obj = _container.Instantiate(_defaultWorldObject);
            var intObj = obj.AddComponent(interactionType) as Interaction;
            _container.Inject(intObj);
            intObj.WorldObject.Init(state.WObject);
            
            IWorldEvent[] events = new IWorldEvent[state.Events.Length];
            IWorldEvent[] consequences = new IWorldEvent[state.Conseqs.Length];
            if (state.Events.Length > 0 && state.Events[0] != null)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    Type eventType = _serializer.WEventTypeById[state.Events[i].EventType];
                    events[i] = _serializer.WEventConstructorByType[eventType].Invoke(state.Events[i], _serializer);
                }
            }
            else events = Array.Empty<IWorldEvent>();

            if (state.Conseqs.Length > 0 && state.Conseqs[0] != null)
            {
                for (int i = 0; i < consequences.Length; i++)
                {
                    Type eventType = _serializer.WEventTypeById[state.Conseqs[i].EventType];
                    consequences[i] = _serializer.WEventConstructorByType[eventType]
                        .Invoke(state.Conseqs[i], _serializer);
                }
            }
            else consequences = Array.Empty<IWorldEvent>();


            intObj.Setup(_serializer.IObjectDataById[state.DataId], events, consequences);
            intObj.Condition = state.Condition;

            // Perhaps move this to a init-like method inside Interaction(or its visual component inside same prefab)
            intObj.GetComponent<SpriteRenderer>().sprite = intObj.Data.Icon;
            
            return intObj;
        }
        
        public GameObject CreateObstacle()
        {
            return Instantiate(_obstacles[Random.Range(0, _obstacles.Length)]);
        }

        public GameObject CreateTile(SurfaceType type)
        {
            GameObject tile = null;
            foreach (var obj in _tiles)
            {
                if (obj == type) tile = obj.Prefab;
            }

            if (tile == null)
                throw new ArgumentException($"Cannot find appropriate prefab for {type} Tile");
            
            return Instantiate(tile);
        }

        public GameObject CreateBorderTile(int type)
        {
            var tile = Instantiate(_borders[type % 2]);
            tile.transform.Rotate(new Vector3(0, 0, 90 * (type / 2)));
            return tile;
        }

        [Serializable]
        private struct TypePrefab<T> where T: Enum
        {
            public GameObject Prefab;
            public T Type;

            public static bool operator ==(TypePrefab<T> a, T b)
            {
                return a.Type.Equals(b);
            }
            public static bool operator !=(TypePrefab<T> a, T b)
            {
                return !(a == b);
            }
        }
    }
}