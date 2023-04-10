using System;
using UnityEngine;

namespace Battle.View.Field
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private CellType _type;

        public void Select()
        {
            _renderer.sprite = FieldView.CellSprites[4];
        }

        public void Deselect()
        {
            _renderer.sprite = FieldView.CellSprites[(int) _type];
        }
        
        public void SetState(CellType type)
        {
            _type = type;
            _renderer.sprite = FieldView.CellSprites[(int) _type];
        }
    }

    public enum CellType
    {
        Normal,
        Passable,
        Ally,
        Enemy,
        Selected
    }
}