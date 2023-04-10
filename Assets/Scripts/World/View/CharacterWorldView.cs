using System;
using DG.Tweening;
using UnityEngine;
using World.Characters;
using World.Objects;

namespace World.View
{
    public class CharacterWorldView : MonoBehaviour
    {
        public event Action FinishedAnimation;
        public event Action StartedAnimation;
        
        [SerializeField] private WorldObject _character;
        [SerializeField] private CharacterMovement _movement;
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2Int _direction;
        [Space] 
        [SerializeField] private float _moveTime;
        
        private static readonly int X = Animator.StringToHash("X");
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int Moving = Animator.StringToHash("IsMoving");

        private void Start()
        {
            _movement.Moved += OnMoved;
        }

        private void OnMoved(Vector2Int direction)
        {
            _direction = direction;
            _animator.SetFloat(X, direction.x);
            _animator.SetFloat(Y, direction.y);
            _animator.SetBool(Moving, true);

            StartedAnimation?.Invoke();
            _character.transform.DOMove(_character.Cell.transform.position, _moveTime)
                .onComplete += FinishMove;
        }

        private void FinishMove()
        {
            _animator.SetBool(Moving, false);
            FinishedAnimation?.Invoke();
        }
    }
}