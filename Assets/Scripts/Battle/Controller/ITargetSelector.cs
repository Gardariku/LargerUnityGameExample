namespace Battle.Controller
{
    // Target Selector holds information, needed to fill data about affected targets in ActionLoop
    // before actually using a skill. Usually it just fires an event and passes itself as parameter, 
    // so that other systems (player input / AI) can deal with target selection based on Selectors type and data.
    public interface ITargetSelector
    {
        public void SelectTarget(BattleController controller, Character actor);
    }
    
    public enum AreaOfEffect
    {
        Single,
        Manhattan,
        Chebyshev,
        Line,
        Cross,
        Cone
    }

    public enum EffectImpact
    {
        None,
        All,
        Enemy,
        Friendly
    }
}