using System.Collections.Generic;
using Battle.Controller;
using Battle.Controller.Events.Input;
using Battle.Controller.Selectors;
using Battle.Data.Skills;
using Battle.View.Field;
using UnityEngine;
using VContainer;

namespace Battle.View
{
    // TODO: Try to fix messy code structure below
    // TODO: Remove selection logic from here?
    public class SelectionView : MonoBehaviour, ITargetSelectionStartedHandler
    {
        private BattleView _battleView;
        private FieldView _fieldView;
        private BattleInput _input;

        private ITargetSelector _selector;
        [SerializeField] private SkillData _skill;
        [SerializeField] private Character _actor;
        private HashSet<Vector2Int> _selectionRange = new ();
        private int _curQuantity;

        [Inject]
        public void Init(BattleView battleView, FieldView fieldView, BattleInput input)
        {
            _battleView = battleView;
            _fieldView = fieldView;
            _input = input;
        }

        private void Start()
        {
            _battleView.Controller.EventBus.Subscribe(this);
            _fieldView.PointerEnteredCell += OnPointerEnteredCell;
            _fieldView.PointerLeftCell += OnPointerLeftCell;
            _fieldView.ClickedOnCell += OnCellClick;
        }

        private void OnCellClick(CellView cell, bool button)
        {
            if (_battleView.State != BattleState.TargetSelection) return;
            
            switch (_selector)
            {
                case CharacterSelector charSel:
                    if (cell.Content == null) return;
                    var character = cell.Content.Character;
                    if (!_selectionRange.Contains(character.Position))
                        return;
                    if ((charSel.TargetRequirements == CharacterSelector.CharacterTarget.Enemy && character.Team == Team.Player)
                        || (charSel.TargetRequirements == CharacterSelector.CharacterTarget.Friendly && character.Team == Team.Enemy))
                        return;

                    _battleView.Controller.ActionLoop.Model.AffectedCharacters.Add(character);
                    _curQuantity++;
                    if (_curQuantity >= charSel.Quantity)
                    {
                        StopSelection();
                        _battleView.Controller.UseSkill(_actor, _skill);
                    }
                    break;
                case CellSelector cellSel:
                    if (!_selectionRange.Contains(cell.GridPos)) return;
                    if (cellSel.Target == CellTarget.Empty && cell.Content != null) return;
                    _battleView.Controller.ActionLoop.Model.AffectedCells.Add(cell.GridPos);
                    _curQuantity++;
                    if (_curQuantity >= cellSel.Quantity)
                    {
                        StopSelection();
                        _battleView.Controller.UseSkill(_actor, _skill);
                    }
                    break;
                default:
                    Debug.LogError($"{_selector.GetType()} selector type handler isn't implemented!");
                    break;
            }
            
            if (_battleView.State == BattleState.TargetSelection && cell.Content != null)
            {
                var character = cell.Content.Character;
                var charSel = (CharacterSelector) _selector;
                if (!_selectionRange.Contains(character.Position))
                    return;
                if ((charSel.TargetRequirements == CharacterSelector.CharacterTarget.Enemy && character.Team == Team.Player)
                    || (charSel.TargetRequirements == CharacterSelector.CharacterTarget.Friendly && character.Team == Team.Enemy))
                    return;

                _battleView.Controller.ActionLoop.Model.AffectedCharacters.Add(character);
                _curQuantity++;
                if (_curQuantity >= charSel.Quantity)
                {
                    StopSelection();
                    _battleView.Controller.UseSkill(_actor, _skill);
                }
            }
        }

        private void OnPointerEnteredCell(CellView cell)
        {
            if (_battleView.State == BattleState.PlayerTurn)
            {
                if (cell.Content != null)
                    cell.Content.Highlight(HighlightType.Info);
                else
                    cell.Select(CellHighlight.Faded);
            }
            else if (_battleView.State == BattleState.TargetSelection)
            {
                switch (_selector)
                {
                    case NoneSelector noSelection:
                        cell.Content?.Highlight(HighlightType.Info);
                        break;
                    case CharacterSelector charSel:
                        if (cell.Content == null)
                            cell.Select(CellHighlight.Faded);
                        else if (_selectionRange.Contains(cell.GridPos))
                        {
                            if (charSel.Impact == EffectImpact.Enemy && cell.Content.Character.Team == Team.Enemy)
                                cell.Content?.Highlight(HighlightType.TargetEnemy);
                            else if (charSel.Impact == EffectImpact.Friendly && cell.Content.Character.Team == Team.Player)
                                cell.Content?.Highlight(HighlightType.TargetAlly);
                            else
                                cell.Content?.Highlight(HighlightType.Info);
                        }
                        else
                            cell.Content?.Highlight(HighlightType.Info);
                        break;
                    case CellSelector cellSel:
                        if (!_selectionRange.Contains(cell.GridPos) 
                            || cellSel.Target == CellTarget.Empty && cell.Content != null)
                            cell.Select(CellHighlight.Faded);
                        else if (_selectionRange.Contains(cell.GridPos))
                            cell.Select(CellHighlight.Selected);
                        break;
                    default:
                        Debug.LogError($"{_selector.GetType()} selector type handler isn't implemented!");
                        break;
                }
            }
        }
        private void OnPointerLeftCell(CellView cell)
        {
            if (cell.Content != null && _selector is not CellSelector)
                cell.Content.StopHighlight();
            else
                cell.Deselect();
        }

        public void CancelSelection()
        {
            _battleView.Controller.ActionLoop.Model.Reset();
            StopSelection();
        }

        public void StopSelection()
        {
            _input.Cancel -= CancelSelection;
            _battleView.ResetState();
            _fieldView.StopHighlightingCells(_selectionRange);
            _selectionRange.Clear();
            _curQuantity = 0;

            _selector = null;
        }

        public void OnTargetSelectionStarted(SkillData skill, Character actor)
        {
            _battleView.State = BattleState.TargetSelection;
            _skill = skill;
            _selector = skill.Selection;
            _actor = actor;

            _input.Cancel += CancelSelection;
            switch (_selector)
            {
                case NoneSelector noSelection:
                    _input.Confirm += ConfirmUsage;
                    break;
                case CharacterSelector charSel:
                    _selectionRange = _fieldView.Field.GetCellsAtArea(AreaOfEffect.Chebyshev, charSel.Range, actor.Position);
                    break;
                case CellSelector cellSel:
                    _selectionRange = _fieldView.Field.GetCellsAtArea(AreaOfEffect.Chebyshev, cellSel.Range, actor.Position);
                    break;
                default:
                    Debug.LogError($"{_selector.GetType()} selector type handler isn't implemented!");
                    break;
            }
            
            _fieldView.HighlightCells(_selectionRange);
        }

        private void ConfirmUsage()
        {
            _battleView.Controller.UseSkill(_actor, _skill);
        }
        
    }
}