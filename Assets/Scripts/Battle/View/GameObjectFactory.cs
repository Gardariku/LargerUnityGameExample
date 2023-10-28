using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Battle.View
{
    // Don't see the need for an actual factory, just use this for newly instantiated gameobjects DI
    public class GameObjectFactory
    {
        [Inject] private IObjectResolver _container;
        
        public GameObject Create(MonoBehaviour monobeh, Transform parent = null)
        {
            var component = _container.Instantiate(monobeh, parent);

            return component.gameObject;
        }
        
        public GameObject Create(GameObject prefab, Transform parent = null)
        {
            var gameObject = _container.Instantiate(prefab, parent);

            return gameObject;
        }
    }
}