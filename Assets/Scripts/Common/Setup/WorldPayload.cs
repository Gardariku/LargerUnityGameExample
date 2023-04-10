using UnityEngine;

namespace Common.Setup
{
    [CreateAssetMenu(fileName = "WorldPayload", menuName = "Scene Payloads / World Payload", order = 0)]
    public class WorldPayload : ScriptableObject
    {
        public string WorldState;
        public LaunchType LaunchType;
        public int ReturnCode = -1;
    }

    public enum LaunchType
    {
        NewGame,
        Load
    }
}