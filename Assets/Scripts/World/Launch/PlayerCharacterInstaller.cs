using UnityEngine;
using World.Characters;
using Zenject;

namespace World.Launch
{
    public class PlayerCharacterInstaller : MonoInstaller
    {
        public Player PlayerObject;

        public override void InstallBindings()
        {
            Container.BindInstance(PlayerObject);
        }
    }
}