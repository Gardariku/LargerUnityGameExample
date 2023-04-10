using World.Objects;
using World.Objects.Interactions;
using Zenject;

namespace World.Launch
{
    public class WorldServicesInitialization : MonoInstaller
    {
        public InteractionCatalog InteractionCatalog;
        public WorldObjectFactory ObjectFactory;
        public WorldSerializer Serializer;
        public WorldGenerator Generator;
        
        public override void InstallBindings()
        {
            Container.BindInstance(InteractionCatalog).AsSingle();
            Container.BindInstance(ObjectFactory).AsSingle();
            Container.BindInstance(Serializer).AsSingle();
            Container.BindInstance(Generator).AsSingle();
        }
    }
}