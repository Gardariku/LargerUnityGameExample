using Common.Setup;
using UnityEngine;
using VContainer;

namespace World.Objects.Interactions
{
    // TODO: look at this after zenject integration
    public class InteractionCatalog : MonoBehaviour
    {
        public SceneLoader SceneLoader { get; private set; }
        [field: SerializeField] public BattlePayload BattlePayload { get; private set; }
        [field: SerializeField] public WorldPayload WorldPayload { get; private set; }
        
        [Inject]
        public void Init(SceneLoader sceneLoader, BattlePayload battlePayload, WorldPayload worldPayload)
        {
            SceneLoader = sceneLoader;
            BattlePayload = battlePayload;
            WorldPayload = worldPayload;
        }

        private void Awake()
        {
            if (SceneLoader == null)
                SceneLoader = FindObjectOfType<SceneLoader>();
        }
    }
}