using Battle.Controller;
using Battle.Data.Characters;
using UnityEngine;
using Zenject;

namespace Battle.View
{
    public class TestBattleLauncher : MonoBehaviour
    {
        private BattleController _controller;
        private BattleView _view;
    
        private FighterData[] _playerTeam;
        private FighterData[] _enemyTeam;

        [Inject]
        public void Init(BattleController battleController, BattleView battleView)
        {
            _controller = battleController;
            _view = battleView;
        }
    
        void Start()
        {
            _controller.Setup(_playerTeam, _enemyTeam);
            _view.Load();
        }
    
    }
}
