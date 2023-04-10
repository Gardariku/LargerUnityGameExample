using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Model;
using Battle.View.Field;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Battle.View
{
    public class CharacterView : MonoBehaviour
    {
        [field: SerializeField] public Character Character { get; private set; }

        public bool Direction => Character.Team == Team.Player;
        public bool IsBusy = false;
        public bool ReachedHitPoint = false;

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Animator _animator;
        [Space]
        [SerializeField] private float _moveTime = 1f;

        private BattleView _battleView;
        private FieldView _fieldView;
        private Material _outlineMaterial;
        private static Dictionary<ShaderProperty, int> shaderProperties = new (){
            {ShaderProperty.Color, Shader.PropertyToID("_OutlineColor")}, 
            {ShaderProperty.Thickness, Shader.PropertyToID("_OutlineThickness")} };

        [Inject]
        public void Init(BattleView battleView, FieldView fieldView)
        {
            _battleView = battleView;
            _fieldView = fieldView;
        }

        public void Load(Character character, BattleView view)
        {
            Character = character;

            // Setting sprite and animations
            _renderer.sprite = character.Data.Sprite;
            _outlineMaterial = new Material(_renderer.material);
            _renderer.material = _outlineMaterial;
            OverrideAnimations();

            // Facing to the right direction
            if (!Direction)
                transform.localScale = new Vector3(-1f, 1f, 1f);
            gameObject.AddComponent<PolygonCollider2D>();
            
            // Subscribe to character-specific events
            character.Moved += OnCharacterMoved;
        }

        private void OnCharacterMoved(Vector2Int newPos)
        {
            _battleView.Controller.Lock();
            _battleView.State = BattleState.Animation;
            transform.DOMove(_fieldView[newPos.x, newPos.y].transform.position, _moveTime)
                .onComplete += () => { _battleView.ResetState(); _battleView.Controller.Unlock(); };
        }

        private void OverrideAnimations()
        {
            var overController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            foreach (var clip in Character.Data.Animations)
                overController[clip.Name.ToString()] = clip.Animation;
            _animator.runtimeAnimatorController = overController;
        }

        public void HighlightTurn()
        {
            shaderProperties.TryGetValue(ShaderProperty.Color, out int color);
            shaderProperties.TryGetValue(ShaderProperty.Thickness, out int thickness);
            _outlineMaterial.SetColor(color, Character.Team == Team.Player ? Color.blue : Color.yellow);
            _outlineMaterial.SetFloat(thickness, 1f);
        }

        public void HighlightTarget()
        {
            shaderProperties.TryGetValue(ShaderProperty.Color, out int color);
            shaderProperties.TryGetValue(ShaderProperty.Thickness, out int thickness);
            if (Character.Team == Team.Player)
                _outlineMaterial.SetColor(color, Color.green);
            else
                _outlineMaterial.SetColor(color, Color.red);
            _outlineMaterial.SetFloat(thickness, 1f);
        }

        public void StopHighlight()
        {
            shaderProperties.TryGetValue(ShaderProperty.Thickness, out int thickness);
            _outlineMaterial.SetFloat(thickness, 0f);
        }

        private void OnMouseEnter()
        {
            if (_battleView.State == BattleState.PlayerTurn && Character.Team != Team.Player)
                HighlightTarget();
        }
        
        private void OnMouseExit()
        {
            if (_battleView.State == BattleState.PlayerTurn && Character.Team != Team.Player)
                StopHighlight();
        }

        private void OnMouseDown()
        {
            if (_battleView.IsPossibleToAttack(Character))
            {
                StopHighlight();
                _battleView.PerformAttack(Character);
            }
        }

        public IEnumerator Attack()
        {
            StopHighlight();

            IsBusy = true;
            _animator.Play("Attack");
            yield return new WaitForEndOfFrame();
            while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                yield return new WaitForEndOfFrame();
            
            IsBusy = false;
            yield return null;
        }
        
        public void OnHitPoint()
        {
            ReachedHitPoint = true;
        }

        // TODO: Find better way to wait for animation end
        public IEnumerator TakeDamage(int damage)
        {
            IsBusy = true;
            _animator.Play("Hurt");
            yield return new WaitForEndOfFrame();
            while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
                yield return new WaitForEndOfFrame();
            
            IsBusy = false;
            yield return null;
        }
        
        public IEnumerator Die()
        {
            _battleView.Controller.Lock();
            while (IsBusy)
                yield return new WaitForEndOfFrame();
            
            IsBusy = true;
            Debug.Log(Character.Data.Name + " has died");
            _animator.SetBool("IsDead", true);
            _animator.Play("Death");
            yield return new WaitForEndOfFrame();
            while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                yield return new WaitForEndOfFrame();
            
            IsBusy = false;
            _battleView.Controller.Unlock();
            yield return null;
        }
    }

    public enum CharacterState
    {
        Idle = 0,
        Turn,
        Animation,
        Target
    }
    
    public enum ShaderProperty
    {
        Color = 0,
        Thickness
    }
}