using Battle.Controller;
using VContainer;
using VContainer.Unity;

namespace Battle.Launch
{
    public class BattleCoreScope : LifetimeScope
    {
        public BattleController BattleController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(BattleController);
        }
    }
}