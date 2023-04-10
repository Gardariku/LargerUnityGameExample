using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.Setup
{
    [CreateAssetMenu(fileName = "BattlePayload", menuName = "Scene Payloads / Battle Payload", order = 0)]
    public class BattlePayload : ScriptableObject
    {
        public List<AssetReference> PlayerCharacters;
        public List<AssetReference> EnemyCharacters;
        public AssetReference BackgroundSprite;
    }
}