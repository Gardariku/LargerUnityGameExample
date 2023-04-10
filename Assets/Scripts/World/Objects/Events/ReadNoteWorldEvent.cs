using System;
using Common.UI;
using UnityEngine;
using World.Objects.Interactions;

namespace World.Objects.Events
{
    [Serializable, AddTypeMenu("Open Notification")]
    public class ReadNoteWorldEvent : IWorldEvent
    {
        [TextArea] 
        [SerializeField] private string _header;
        [TextArea] 
        [SerializeField] private string _message;
        
        public void Activate(Interaction context)
        {
            UIRouter.OpenUrl($"info?header={_header}&message={_message}");
        }

        public ReadNoteWorldEvent(string header, string message)
        {
            _header = header;
            _message = message;
        }

        public WorldEventState GetState(WorldSerializer serializer)
        {
            return new ReadNoteState(this, serializer);
        }
        
        [Serializable]
        public class ReadNoteState : WorldEventState
        {
            public string Header;
            public string Message;

            public ReadNoteState(ReadNoteWorldEvent worldEvent, WorldSerializer serializer) 
                : base(worldEvent, serializer)
            {
                Header = worldEvent._header;
                Message = worldEvent._message;
            }
        }
    }
}