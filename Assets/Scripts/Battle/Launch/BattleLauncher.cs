using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battle.Controller;
using Battle.Data.Characters;
using Battle.View;
using Common.Setup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;

namespace Battle.Launch
{
    public class BattleLauncher : IStartable
    {
        private SceneLoader _sceneLoader;
        private BattlePayload _battlePayload;
        private WorldPayload _worldPayload;
        
        private BattleController _controller;
        private BattleView _view;
    
        [SerializeField] private FighterData[] _playerTroops;
        [SerializeField] private FighterData[] _enemyTroops;

        private AsyncOperationHandle<IList<FighterData>> _playerHandle;
        private AsyncOperationHandle<IList<FighterData>> _enemyHandle;
        private AsyncOperationHandle<Sprite> _backgroundHandle;

        [Inject]
        public void Init(BattlePayload battlePayload, WorldPayload worldPayload, SceneLoader sceneLoader,
            BattleController battleController, BattleView battleView)
        {
            _battlePayload = battlePayload;
            _worldPayload = worldPayload;
            _sceneLoader = sceneLoader;
            _controller = battleController;
            _view = battleView;
        }

        public async void Start()
        {
            _playerHandle = Addressables.LoadAssetsAsync<FighterData>(_battlePayload.PlayerCharacters, null, Addressables.MergeMode.Union);
            _enemyHandle = Addressables.LoadAssetsAsync<FighterData>(_battlePayload.EnemyCharacters, null, Addressables.MergeMode.Union);
            _backgroundHandle = Addressables.LoadAssetAsync<Sprite>(_battlePayload.BackgroundSprite);
            await Task.WhenAll(_playerHandle.Task, _enemyHandle.Task, _backgroundHandle.Task);

            _playerTroops = _playerHandle.Result.ToArray();
            _enemyTroops = _enemyHandle.Result.ToArray();
            
            _controller.Setup(_playerTroops, _enemyTroops);
            _view.Load();

            _controller.GameStateEvents.GameEnded += CloseGame;
        }

        private void CloseGame(Team winner)
        {
            _worldPayload.ReturnCode = (int) winner - 1;
            _worldPayload.LaunchType = LaunchType.Load;
            _sceneLoader.LoadSceneSingle(SceneType.World);
        }
    }
}