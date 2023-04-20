using UnityEngine;
using Zenject;

namespace Battle.View.Field
{
    // TODO: Add an abstraction layer in place of CharacterView (FieldObject or smth)
    [RequireComponent(typeof(SpriteRenderer))]
    public class CellView : MonoBehaviour
    {
        [field: SerializeField] public CharacterView Content { get; set; }
        [field: SerializeField] public Vector2Int GridPos { get; set; }
        [SerializeField] private CellHighlight _highlight;
        [SerializeField] private SpriteRenderer _renderer;
        private FieldView _fieldView;

        [Inject]
        public void Init(FieldView fieldView)
        {
            _fieldView = fieldView;
        }

        public void Select(CellHighlight type)
        {
            _renderer.sprite = FieldView.CellSprites[(int) type];
        }
        public void Deselect()
        {
            _renderer.sprite = FieldView.CellSprites[(int) _highlight];
        }
        
        public void SetState(CellHighlight highlight)
        {
            _highlight = highlight;
            _renderer.sprite = FieldView.CellSprites[(int) _highlight];
        }
        public void SetDefault()
        {
            SetState(CellHighlight.Normal);
        }

        private void OnMouseOver()
        {
            _fieldView.PointerEnteredCell?.Invoke(this);
        }
        
        private void OnMouseExit()
        {
            _fieldView.PointerLeftCell?.Invoke(this);
        }

        private void OnMouseDown()
        {
            _fieldView.ClickedOnCell?.Invoke(this, Input.GetKeyDown(KeyCode.Mouse1));
        }
    }

    public enum CellHighlight
    {
        Normal,
        Passable,
        Ally,
        Enemy,
        Selected,
        Faded
    }
}