using System;
using UnityEngine;
using VContainer;

namespace Battle.View
{
    // TODO: Replace with new input system later
    public class BattleInput : MonoBehaviour
    {
        public event Action Confirm;
        public event Action Cancel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Confirm?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel?.Invoke();
            }
        }
    }
}