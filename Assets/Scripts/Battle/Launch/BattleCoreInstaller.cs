using Battle.Controller;
using Battle.View;
using Zenject;

namespace Battle.Launch
{
    public class BattleCoreInstaller : MonoInstaller
    {
        public BattleController BattleController;
        
        public override void InstallBindings()
        {
            Container.BindInstance(BattleController).AsSingle();
            Container.Bind<GameObjectFactory>().FromNew().AsSingle();
        }
    }
}