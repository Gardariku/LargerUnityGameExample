using UnityEngine;
using VContainer;
using World.Characters;

namespace World.View
{
    public class PlayerInput : MonoBehaviour
    {
        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";

        private Player _player;
        [SerializeField] private bool _blockInput;

        [Inject]
        public void Init(Player player)
        {
            _player = player;
        }

        private void Start()
        {
            _player.Visual.StartedAnimation += () => _blockInput = true;
            _player.Visual.FinishedAnimation += () => _blockInput = false;
        }

        private void Update()
        {
            if (_blockInput)
                return;
            
            var direction = new Vector2(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis));
            if (direction != Vector2.zero)
                _player.Movement.TryMove(Vector2Int.FloorToInt(direction));

            if (Input.GetKeyDown(KeyCode.E))
                _player.Interaction.TryInteract();
        }
    }
}