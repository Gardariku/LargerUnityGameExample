using System;
using System.Collections.Generic;
using Battle.Controller;
using Battle.Controller.Field;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Battle.View.Field
{
    public class FieldView : MonoBehaviour
    {
        private BattleView _battleView;
        private GameObjectFactory _factory;
        private Camera _normalCamera;
        private Tilemap _tilemap;
        [Space] 
        private CellView _cellPrefab;
        [SerializeField] private CellView[] _cells;
        [SerializeField] private Sprite[] _cellSprites;

        private const float CellSize = 100f;
        private int _width;
        public BattleField Field { get; private set; }
        private Vector3 _mousePos;
        private Vector3Int _tilePos;
        private CellView _currentTile;

        public CellView this[int i, int j] => _cells[i + j * _width];
        
        public static Sprite[] CellSprites { get; private set; }

        // left button = false, right button = true
        public Action<CellView, bool> ClickedOnCell;
        public Action<CellView> PointerEnteredCell;
        public Action<CellView> PointerLeftCell;

        [Inject]
        public void Init(Tilemap tilemap, CellView cellView, Camera mainCamera, BattleView battleView, GameObjectFactory factory)
        {
            _battleView = battleView;
            _normalCamera = mainCamera;
            _tilemap = tilemap;
            _cellPrefab = cellView;
            _factory = factory;
        }

        public void Setup(BattleField field)
        {
            SetCells();
            Field = field;
            Vector3 fieldPosition = _tilemap.transform.position;
            _width = field.Size.x;
            _tilemap.transform.position = new (fieldPosition.x - (_width / 2 - 0.5f) * CellSize, fieldPosition.y);
            _cells = new CellView[field.Size.x * field.Size.y];
            
            for (int i = 0; i < field.Size.x; i++)
            {
                for (int j = 0; j < field.Size.y; j++)
                {
                    _cells[i + j * _width] = _factory.Create(_cellPrefab, _tilemap.transform).GetComponent<CellView>();
                    _cells[i + j * _width].transform.position = _tilemap.CellToWorld(new(i, j));
                    _cells[i + j * _width].gameObject.name = $"Cell ({i};{j})";
                    _cells[i + j * _width].GridPos = new(i, j);
                }
            }
        }

        public void HighlightCells(IEnumerable<Vector2Int> positions)
        {
            foreach (var pos in positions)
            {
                var cell = this[pos.x, pos.y];
                if (cell.Content == null)
                    cell.SetState(CellHighlight.Passable);
                else 
                    cell.SetState(cell.Content.Character.Team == Team.Player ? CellHighlight.Ally : CellHighlight.Enemy);
            }
        }
        public void StopHighlightingCells(IEnumerable<Vector2Int> positions)
        {
            foreach (var pos in positions)
            {
                this[pos.x, pos.y].SetDefault();
            }
        }

        // TODO: Move input to individual script and clean it
        // void Update()
        // {
        //     if (_battleView.State != BattleState.PlayerTurn) return;
        //
        //     var hit = Physics2D.GetRayIntersection(
        //         new Ray(Input.mousePosition + Vector3.back * 10f, Vector3.forward), 30f);
        //     if (hit.collider != null && hit.collider.TryGetComponent(out CellView newTile))
        //     {
        //         CheckClick();
        //
        //         if (_currentTile == newTile)
        //             return;
        //
        //         newTile.Select();
        //         _currentTile?.Deselect();
        //         _currentTile = newTile;
        //         return;
        //     }
        //     _currentTile?.Deselect();
        // }
        //
        // private void CheckClick()
        // {
        //     if (Input.GetKeyUp(KeyCode.Mouse0) && _battleView.State == BattleState.PlayerTurn)
        //     {
        //         _mousePos = _normalCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 50f, Input.mousePosition.y + 50f, -10f));
        //         _tilePos = _tilemap.WorldToCell(_mousePos);
        //         ClickedOnCell?.Invoke(new (_tilePos.x, _tilePos.y));
        //     }
        // }

        private void SetCells()
        {
            CellSprites = new Sprite[_cellSprites.Length];
            for (int i = 0; i < _cellSprites.Length; i++)
                CellSprites[i] = _cellSprites[i];
        }
    }
}