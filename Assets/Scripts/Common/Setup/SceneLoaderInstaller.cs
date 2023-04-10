using System.ComponentModel;
using Zenject;

namespace Common.Setup
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        public SceneLoader SceneLoader;

        public override void InstallBindings()
        {
            var loader = Instantiate(SceneLoader);
            DontDestroyOnLoad(loader);
            Container.Bind<SceneLoader>().FromInstance(loader).AsSingle();
            Container.QueueForInject(loader);
        }
    }
}