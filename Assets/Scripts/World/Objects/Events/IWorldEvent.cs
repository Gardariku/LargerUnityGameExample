using System;
using UnityEngine;
using World.Objects.Interactions;

namespace World.Objects.Events
{
    public interface IWorldEvent
    {
        public void Activate(Interaction context);
        public WorldEventState GetState(WorldSerializer serializer);
    }

    [Serializable]
    public class WorldEventState
    {
        public int EventType;

        public WorldEventState(IWorldEvent worldEvent, WorldSerializer serializer)
        {
            EventType = serializer.WEventIdByType[worldEvent.GetType()];
        }
    }
}