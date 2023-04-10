using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace World.Launch
{
    public class WorldMapInitializer : MonoInstaller
    {
        public WorldMap Map;
        public Tilemap Tilemap;
        public Camera Camera;

        public override void InstallBindings()
        {
            Container.BindInstance(Map).AsSingle();
            Container.BindInstance(Tilemap).AsSingle();
            Container.BindInstance(Camera).AsSingle();
        }
    }
}