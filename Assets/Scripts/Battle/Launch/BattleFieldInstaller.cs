using Battle.View.Field;
using UnityEngine.Tilemaps;
using Zenject;

namespace Battle.Launch
{
    // This is probably excessive at this point, so mostly just experimenting here
    public class BattleFieldInstaller : MonoInstaller
    {
        public Tilemap Tilemap;
        public CellView CellPrefab;
        
        public override void InstallBindings()
        {
            Container.BindInstance(Tilemap).AsSingle();
            Container.BindInstance(CellPrefab).AsCached();
        }
    }
}