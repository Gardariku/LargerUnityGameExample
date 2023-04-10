using UnityEngine;
using Zenject;

namespace Battle.View
{
    // Don't see the need for an actual factory, just use this for newly instantiated gameobjects DI
    public class GameObjectFactory
    {
        [Inject] private DiContainer _container;
        
        public GameObject Create(MonoBehaviour monobeh, Transform parent = null)
        {
            var gameObject = _container.InstantiatePrefab(monobeh, parent);

            return gameObject;
        }
        
        public GameObject Create(GameObject prefab, Transform parent = null)
        {
            var gameObject = _container.InstantiatePrefab(prefab, parent);

            return gameObject;
        }
    }
}