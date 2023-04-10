using System.Collections;

namespace Battle.Controller
{
    public abstract class GameplayLoop<T>
    {
        public T Model { get; set; }
        protected BattleController _controller;

        public abstract IEnumerator Start();
        
        protected GameplayLoop(BattleController controller)
        {
            _controller = controller;
        }
    }
}