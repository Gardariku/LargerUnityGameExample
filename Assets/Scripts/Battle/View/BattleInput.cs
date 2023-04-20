using System;
using UnityEngine;
using Zenject;

namespace Battle.View
{
    // TODO: Replace with new input system later
    public class BattleInput : MonoBehaviour
    {
        private BattleView _battleView;
        private SelectionView _selectionView;

        public event Action Confirm;
        public event Action Cancel;

        [Inject]
        public void Init(BattleView battleView, SelectionView selectionView)
        {
            _battleView = battleView;
            _selectionView = selectionView;
        }
        
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