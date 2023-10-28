using VContainer;
using VContainer.Unity;

namespace Common.Setup
{
    public class ProjectScope : LifetimeScope
    {
        public SceneLoader SceneLoaderPrefab;
        public WorldPayload World;
        public BattlePayload Battle;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(SceneLoaderPrefab, Lifetime.Singleton);

            builder.RegisterInstance(World);
            builder.RegisterInstance(Battle);
        }
    }
}