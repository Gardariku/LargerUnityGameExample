using Zenject;

namespace Common.Setup
{
    public class ScenePayloadsInstaller : MonoInstaller<ScenePayloadsInstaller>
    {
        public WorldPayload World;
        public BattlePayload Battle;
        
        public override void InstallBindings()
        {
            Container.BindInstance(World);
            Container.BindInstance(Battle);
        }
    }
}