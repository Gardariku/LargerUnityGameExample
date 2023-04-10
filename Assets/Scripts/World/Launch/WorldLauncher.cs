using System;
using Common.Setup;
using UnityEngine;
using Zenject;

namespace World.Launch
{
    public class WorldLauncher : MonoBehaviour
    {
        private WorldPayload _payload;
        private WorldMap _map;
        private WorldSerializer _serializer;

        [Inject]
        public void Init(WorldPayload worldPayload, WorldMap map, WorldSerializer serializer)
        {
            _payload = worldPayload;
            _map = map;
            _serializer = serializer;
        }

        private void Start()
        {
            switch (_payload.LaunchType)
            {
                case LaunchType.NewGame:
                    _map.GenerateNewMap();
                    break;
                case LaunchType.Load:
                    _map.LoadMap(_serializer.Deserialize(_payload.WorldState));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}