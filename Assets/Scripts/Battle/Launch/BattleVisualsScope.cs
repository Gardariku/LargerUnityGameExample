using Battle.View;
using Battle.View.Field;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;
using VContainer.Unity;

namespace Battle.Launch
{
    public class BattleVisualsScope : LifetimeScope
    {
        public Camera MainCamera;
        public BattleView BattleView;
        public FieldView FieldView;
        public SelectionView SelectionView;
        public BattleInput Input;
        public Tilemap Tilemap;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(MainCamera);
            builder.RegisterComponent(BattleView);
            builder.RegisterComponent(FieldView);
            builder.RegisterComponent(SelectionView);
            builder.RegisterComponent(Input);
            builder.RegisterComponent(Tilemap);
            
            builder.Register<GameObjectFactory>(Lifetime.Singleton);

            builder.RegisterEntryPoint<BattleLauncher>();
        }
    }
}