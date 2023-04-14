using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Data;
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

        private static readonly int IsBusy = Animator.StringToHash("IsBusy");

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
            character.Events.Moved += OnCharacterMoved;
            character.Events.Attacked += anim => StartCoroutine(PlayAnimation(anim));
            character.Events.DamageTaken += _ => StartCoroutine(PlayAnimation(BattleAnimation.Hurt));
        }

        private void OnCharacterMoved(Vector2Int newPos)
        {
            _battleView.Controller.Lock();
            transform.DOMove(_fieldView[newPos.x, newPos.y].transform.position, _moveTime)
                .onComplete += () => _battleView.Controller.Unlock();
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

        
        // TODO: Try calling this via inspector reference (replace collection of clips with overriden controllers)
        // Supposed to be called from the animation events
        public void SetBusy(int value)
        {
            _animator.SetBool(IsBusy, value > 0);
        }

        public IEnumerator PlayAnimation(BattleAnimation animation)
        {
            StopHighlight();
            _battleView.Controller.Lock();
            string animationName = animation.ToString();
            
            _animator.SetBool(IsBusy, true);
            _animator.Play(animationName);
            yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => _animator.GetBool(IsBusy));

            _battleView.Controller.Unlock();
        }
    }

    public enum ShaderProperty
    {
        Color = 0,
        Thickness
    }
}