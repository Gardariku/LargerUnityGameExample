using System;
using System.Collections.Generic;
using Common.Setup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using World.Objects.Interactions;

namespace World.Objects.Events
{
    [Serializable, AddTypeMenu("Start Battle")]
    public class StartBattleWorldEvent : IWorldEvent
    {
        [SerializeField] private List<AssetReference> _enemies;
        [SerializeField] private AssetReference _background;
        
        public StartBattleWorldEvent() { }

        public void Activate(Interaction context)
        {
            context.Catalog.BattlePayload.EnemyCharacters = _enemies;
            context.Catalog.BattlePayload.BackgroundSprite = _background;
            context.Catalog.SceneLoader.LoadSceneSingle(SceneType.Battle);
        }

        public WorldEventState GetState(WorldSerializer serializer)
        {
            return new StartBattleState(this, serializer);
        }

        public StartBattleWorldEvent(List<AssetReference> enemies, AssetReference background)
        {
            _enemies = enemies;
            _background = background;
        }
        
        [Serializable]
        public class StartBattleState : WorldEventState
        {
            public int[] Enemies;
            public int BG;

            public StartBattleState(StartBattleWorldEvent worldEvent, WorldSerializer serializer) 
                : base(worldEvent,serializer)
            {
                Enemies = new int[worldEvent._enemies.Count];
                for (int i = 0; i < worldEvent._enemies.Count; i++)
                    Enemies[i] = serializer.EnemyIdByWeakRef[worldEvent._enemies[i].RuntimeKey];
                BG = serializer.BackgroundIdByWeakRef[worldEvent._background.RuntimeKey];
            }
        }
    }
}