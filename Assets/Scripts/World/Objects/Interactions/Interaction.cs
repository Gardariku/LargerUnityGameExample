using System;
using UnityEngine;
using World.Data;
using World.Objects.Events;
using Zenject;

namespace World.Objects.Interactions
{
    [RequireComponent(typeof(WorldObject))]
    public abstract class Interaction : MonoBehaviour
    {
        [field: SerializeField] public InteractableObjectData Data { get; private set; }
        [field: SerializeField] public WorldObject WorldObject { get; private set; }
        [field: Space]
        [field: SerializeField] public int Condition { get; set; }
        [SerializeReference, SubclassSelector] private IWorldEvent[] _events;
        [SerializeReference, SubclassSelector] private IWorldEvent[] _consequences;
        public InteractionCatalog Catalog { get; private set; }

        private void Awake()
        {
            WorldObject = GetComponent<WorldObject>();
        }

        [Inject]
        public void Init(InteractionCatalog catalog)
        {
            Catalog = catalog;
        }

        public void Setup(InteractableObjectData data, 
            IWorldEvent[] events, IWorldEvent[] conseqs)
        {
            Data = data;
            _events = events;
            _consequences = conseqs;
        }

        public void Interact()
        {
            if (Condition >= 0)
                _events[Condition].Activate(this);
        }

        public void AcceptResult(int code)
        {
            if (code >= 0 && _consequences.Length > code)
                _consequences[code].Activate(this);
        }
        
        // Tried to avoid memallocks here, but EventStates being a value type seems like too much of a trouble
        // Probably still can be implemented in some way though
        [Serializable]
        public class InteractableObjectState
        {
            public int InteractionType;
            public int DataId;
            public WorldObjectState WObject;
            public WorldEventState[] Events;
            public WorldEventState[] Conseqs;
            public int Condition;

            public InteractableObjectState(Interaction intObj, WorldSerializer serializer)
            {
                InteractionType = serializer.InteractionIdByType[intObj.GetType()];
                DataId = serializer.IObjectIdByData[intObj.Data];
                WObject = new WorldObjectState(intObj.WorldObject);
                Condition = intObj.Condition;

                if (intObj._events.Length > 0)
                {
                    Events = new WorldEventState[intObj._events.Length];
                    for (int i = 0; i < intObj._events.Length; i++)
                        Events[i] = intObj._events[i].GetState(serializer);
                }
                else Events = Array.Empty<WorldEventState>();
                if (intObj._consequences.Length > 0)
                {
                    Conseqs = new WorldEventState[intObj._consequences.Length];
                    for (int i = 0; i < intObj._consequences.Length; i++)
                        Conseqs[i] = intObj._consequences[i].GetState(serializer);
                }
                else Conseqs = Array.Empty<WorldEventState>();

            }
        }
    }
}