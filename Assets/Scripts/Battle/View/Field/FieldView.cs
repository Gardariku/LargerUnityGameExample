using Battle.Controller;
using Battle.Controller.Commands;
using Battle.Controller.Field;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Battle.View.Field
{
    public class FieldView : MonoBehaviour
    {
        private BattleView _battleView;
        private Camera _normalCamera;
        private Tilemap _tilemap;
        [Space] 
        private CellView _cellPrefab;
        [SerializeField] private CellView[] _cells;
        [SerializeField] private Sprite[] _cellSprites;

        private const float CellSize = 100f;
        private int _width;
        private BattleField _field;
        private Vector3 _mousePos;
        private Vector3Int _tilePos;
        private CellView _currentTile;

        public CellView this[int i, int j] => _cells[i + j * _width];
        
        public static Sprite[] CellSprites { get; private set; }

        [Inject]
        public void Init(Tilemap tilemap, CellView cellView, Camera mainCamera, BattleView battleView)
        {
            _battleView = battleView;
            _normalCamera = mainCamera;
            _tilemap = tilemap;
            _cellPrefab = cellView;
        }

        public void Setup(BattleField field)
        {
            SetCells();
            _field = field;
            Vector3 fieldPosition = _tilemap.transform.position;
            _width = field.Size.x;
            _tilemap.transform.position = new (fieldPosition.x - (_width / 2 - 0.5f) * CellSize, fieldPosition.y);
            _cells = new CellView[field.Size.x * field.Size.y];
            
            for (int i = 0; i < field.Size.x; i++)
            {
                for (int j = 0; j < field.Size.y; j++)
                {
                    _cells[i + j * _width] = Instantiate(_cellPrefab, _tilemap.transform);
                    _cells[i + j * _width].transform.position = _tilemap.CellToWorld(new(i, j));
                    _cells[i + j * _width].gameObject.name = $"Cell ({i};{j})";
                }
            }

            _battleView.Controller.GameStateEvents.CharacterTurnStarted += OnCharacterTurnStarted;
            _battleView.Controller.GameStateEvents.CharacterTurnEnded += OnCharacterTurnFinished;
            _battleView.Controller.CharacterEvents.CharacterMoveStarted += OnCharacterMoveStarted;
            _battleView.Controller.CharacterEvents.CharacterMoveFinished += OnCharacterMoveFinished;
        }

        private void OnCharacterMoveStarted(MoveCharacterCommand moveCommand)
        {
            OnCharacterTurnFinished(moveCommand.Actor);
        }
        private void OnCharacterMoveFinished(MoveCharacterCommand moveCommand)
        {
            OnCharacterTurnStarted(moveCommand.Actor);
        }

        private void OnCharacterTurnStarted(Character character)
        {
            if (character.Team != Team.Player) return;

            foreach (var pos in _field.GetPassableCells(character))
                this[pos.x, pos.y].SetState(CellType.Passable);

            _field.GetCharactersCells(out var allies, out var enemies);
            foreach (var pos in allies)
                this[pos.x, pos.y].SetState(CellType.Ally);
            foreach (var pos in enemies)
                this[pos.x, pos.y].SetState(CellType.Enemy);
        }
        private void OnCharacterTurnFinished(Character character)
        {
            if (character.Team != Team.Player) return;

            foreach (var cell in _cells)
                cell.SetState(CellType.Normal);
        }

        // TODO: Move input to individual script and clean it
        void Update()
        {
            if (_battleView.State != BattleState.PlayerTurn) return;
            // _mousePos = _normalCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 50f, Input.mousePosition.y + 50f, -10f));
            // _tilePos = _tilemap.WorldToCell(_mousePos);
            // if (_tilePos.x < 0 || _tilePos.x >= _field.Size.x || _tilePos.y < 0 || _tilePos.y >= _field.Size.y)
            //     return;
            // var newTile = this[_tilePos.x, _tilePos.y];
            
            var hit = Physics2D.GetRayIntersection(new Ray(Input.mousePosition + Vector3.back * 10f, Vector3.forward), 30f);

            if (hit.collider != null && hit.collider.TryGetComponent(out CellView newTile))
            {
                CheckClick();

                if (_currentTile == newTile)
                    return;

                newTile.Select();
                _currentTile?.Deselect();
                _currentTile = newTile;
                return;
            }
            _currentTile?.Deselect();
        }

        private void CheckClick()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) && _battleView.State == BattleState.PlayerTurn)
            {
                _mousePos = _normalCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 50f, Input.mousePosition.y + 50f, -10f));
                _tilePos = _tilemap.WorldToCell(_mousePos);
                _battleView.Controller.TryMove(_battleView.CurrentCharacter, new(_tilePos.x, _tilePos.y));
            }
        }

        private void SetCells()
        {
            CellSprites = new Sprite[_cellSprites.Length];
            for (int i = 0; i < _cellSprites.Length; i++)
                CellSprites[i] = _cellSprites[i];
        }
    }
}