using System;

namespace Battle.Controller.Selectors
{
    [Serializable, AddTypeMenu("No Selection")]
    public class NoneSelector : ITargetSelector
    {
        public NoneSelector() {}
        
        public void SelectTarget(BattleController controller, Character actor)
        {
        }
    }
}