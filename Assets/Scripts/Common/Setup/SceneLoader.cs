using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using VContainer;

namespace Common.Setup
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference[] _scenes;
        private WorldPayload _worldPayload;
        private SceneType _currentSceneType;
        private AsyncOperationHandle<SceneInstance> _handle;

        public event Action<SceneType> LoadingScene;

        [Inject]
        public void Init(WorldPayload worldPayload)
        {
            _worldPayload = worldPayload;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void LoadSceneSingle(SceneType type)
        {
            LoadingScene?.Invoke(type);
            _currentSceneType = type;
            var scene = _scenes[(int) type];
            Addressables.LoadSceneAsync(scene)
                    .Completed += SceneLoadCompleted;
        }

        private void SceneLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Successfully loaded {_currentSceneType} scene.");
                _handle = obj;
            }
        }

        public void UnloadScene()
        {
            Addressables.UnloadSceneAsync(_handle, true).Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded) Debug.Log($"Successfully unloaded {_currentSceneType} scene.");
            };
        }

        private void OnApplicationQuit()
        {
            _worldPayload.LaunchType = LaunchType.NewGame;
        }
    }

    public enum SceneType
    {
        Main,
        World,
        Battle
    }
}