using UnityEngine;
using VContainer;
using World.Characters;

namespace World.View
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        private WorldMap _worldMap;
        private CharacterWorldView _player;
        [SerializeField] private float _moveSpeed  = 8f;
        [SerializeField] private float _zoomSpeed  = 5f;
        [SerializeField] private float _zoomFactor = 3f;
        [SerializeField] private float _bufferLength = 20f;
        [SerializeField] private bool _isAttached;

        private float borderMargin = 0f;
        private float leftBorder;
        private float rigthBorder;
        private float upperBorder;
        private float bottomBorder;

        private Camera _camera;
        private float targetZoom;

        [Inject]
        public void Init(WorldMap worldMap, Player player)
        {
            _worldMap = worldMap;
            _player = player.Visual;
        }

        private void Awake()
        {
            _worldMap.CreatedWorld += Setup;
            _camera = GetComponent<Camera>();
        }

        private void Setup()
        {
            targetZoom = _camera.orthographicSize;

            leftBorder = -borderMargin;
            rigthBorder = _worldMap.Width + borderMargin;
            upperBorder = _worldMap.Height + borderMargin;
            bottomBorder = -borderMargin;
            
            transform.position = new (_player.transform.position.x, _player.transform.position.y, transform.position.z);
            _player.StartedAnimation += () => _isAttached = true;
            _player.FinishedAnimation += () => _isAttached = false;
        }
        
        private void Update()
        {
            if (_isAttached)
                return;
            
            if (Input.mousePosition.x >= Screen.width - _bufferLength && transform.position.x < rigthBorder)
            {
                transform.position += Vector3.right * Time.deltaTime * _moveSpeed;
            }
            if (Input.mousePosition.y >= Screen.height - _bufferLength && transform.position.y < upperBorder)
            {
                transform.position += Vector3.up * Time.deltaTime * _moveSpeed;
            }
            if (Input.mousePosition.x <= _bufferLength && transform.position.x > leftBorder)
            {
                transform.position += Vector3.left * Time.deltaTime * _moveSpeed;
            }
            if (Input.mousePosition.y <= _bufferLength && transform.position.y > bottomBorder)
            {
                transform.position += Vector3.down * Time.deltaTime * _moveSpeed;
            }

            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (!scrollData.Equals(0))
            {
                targetZoom -= scrollData * _zoomFactor;
                targetZoom = Mathf.Clamp(targetZoom, 4f, 12f);
            }     
        }

        private void LateUpdate()
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetZoom, Time.deltaTime * _zoomSpeed);

            if (_isAttached)
                transform.position = new (_player.transform.position.x, _player.transform.position.y, transform.position.z);
        }
    }
}