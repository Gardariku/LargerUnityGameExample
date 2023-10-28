using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;
using VContainer.Unity;
using World.Characters;
using World.Objects;
using World.Objects.Interactions;
using World.View;

namespace World.Launch
{
    public class WorldScope : LifetimeScope
    {
        public InteractionCatalog InteractionCatalog;
        public WorldSerializer Serializer;
        public WorldGenerator Generator;
        [Space]
        public WorldLauncher Launcher;
        public CameraMovement CameraMovement;
        [Space]
        public WorldMap Map;
        public Tilemap Tilemap;
        public WorldObjectFactory ObjectFactory;
        public Camera Camera;
        [Space]
        public Player PlayerObject;
        public PlayerInput Input;
        public CharacterMovement Movement;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(InteractionCatalog);
            builder.RegisterComponent(Serializer);
            builder.RegisterComponent(Generator);
            
            builder.RegisterComponent(Map);
            builder.RegisterComponent(ObjectFactory);
            builder.RegisterComponent(Tilemap);
            builder.RegisterComponent(Camera);
            
            builder.RegisterComponent(PlayerObject);
            
            builder.RegisterBuildCallback(OnBuildCallback);
        }

        private void OnBuildCallback(IObjectResolver container)
        {
            container.Inject(Launcher);

            foreach (var interaction in FindObjectsOfType<Interaction>())
                container.Inject(interaction);
            
            container.Inject(CameraMovement);
            
            container.Inject(Input);
            container.Inject(Movement);
        }
    }
}