using System;
using World.Objects.Interactions;

namespace World.Objects.Events
{
    [Serializable, AddTypeMenu("Disable Object")]
    public class DisableObjectWorldEvent : IWorldEvent
    {
        public DisableObjectWorldEvent() { }
        
        public void Activate(Interaction context)
        {
            context.Condition = -1;
        }

        public WorldEventState GetState(WorldSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}