using Battle.View;
using Battle.View.Field;
using UnityEngine;
using Zenject;

namespace Battle.Launch
{
    public class BattleVisualsInstaller : MonoInstaller
    {
        public Camera MainCamera;
        public BattleView BattleView;
        public FieldView FieldView;
        public CharacterView CharacterPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(MainCamera).AsCached();
            Container.BindInstance(BattleView).AsSingle();
            Container.BindInstance(FieldView).AsSingle();
            Container.Bind<CharacterView>().FromComponentOn(CharacterPrefab.gameObject).AsCached();
        }
    }
}