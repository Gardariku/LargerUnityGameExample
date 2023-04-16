using System;
using World.Objects.Interactions;

namespace World.Objects.Events
{
    [Serializable, AddTypeMenu("Get Items")]
    public class GetItemWorldEvent : IWorldEvent
    {
        public GetItemWorldEvent() {}
        
        public void Activate(Interaction context)
        {
            throw new System.NotImplementedException();
        }

        public WorldEventState GetState(WorldSerializer serializer)
        {
            return null;
        }
    }
}