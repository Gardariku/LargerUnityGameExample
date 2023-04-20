using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Controller;
using Battle.Data;
using Battle.View.Field;
using UnityEngine;

using Zenject;

namespace Battle.View
{
    public class BattleView : MonoBehaviour
    {
        // [field: SerializeField] for auto properties
        public BattleState State 
        { get => _state; set { _prevState = _state; _state = value; } }
        [SerializeField] private BattleState _state;
        private BattleState _prevState;
        [field: SerializeField] public Character CurrentCharacter { get; set; }
        public BattleController Controller { get; private set; }
        public FieldView FieldView { get; private set; }
        [SerializeField] private List<CharacterView> _characters;
        private CharacterView _characterPrefab;
        private GameObjectFactory _objectFactory;

        public Action<CharacterView> ClickedOnCharacter;
        public Action<CharacterView> PointerEnteredCharacter;
        public Action<CharacterView> PointerLeftCharacter;

        [Inject]
        public void Init(BattleController battleController, FieldView fieldView, CharacterView characterView,
            GameObjectFactory objectFactory)
        {
            Controller = battleController;
            FieldView = fieldView;
            _characterPrefab = characterView;
            _objectFactory = objectFactory;
        }
        
        public void Load()
        {
            SubscribeOnGameEvents();
            SubscribeOnCharacterEvents();
            FieldView.Setup(Controller.BattleModel.Field);
            LoadCharacters(Controller.BattleModel.Characters);
        }
        
        private void SubscribeOnGameEvents()
        {
            Controller.GameStateEvents.GameStarted += OnBattleStarted;
            Controller.GameStateEvents.RoundStarted += OnRoundStarted;
            Controller.GameStateEvents.CharacterTurnStarted += OnCharacterTurnStarted;
            Controller.GameStateEvents.CharacterTurnEnded += OnCharacterTurnFinished;
            Controller.GameStateEvents.RoundEnded += OnRoundFinished;
            Controller.GameStateEvents.GameEnded += OnBattleFinished;
        }
        private void SubscribeOnCharacterEvents()
        {
            Controller.CharacterEvents.CharacterActionStarted += OnCharacterAnimationStarted;
            Controller.CharacterEvents.CharacterActionFinished += OnCharacterAnimationFinished;
            Controller.CharacterEvents.CharacterDeathFinished += OnCharacterDied;
        }

        private void LoadCharacters(List<Character> characters)
        {
            foreach (var character in characters)
            {
                InstantiateCharacter(character);
            }
        }
        private void InstantiateCharacter(Character character)
        {
            var pos = character.Position;
            Transform transform = FieldView[pos.x, pos.y].transform;
            CharacterView characterView = _objectFactory.Create(_characterPrefab, transform).GetComponent<CharacterView>();
            characterView.Load(character, this);
            _characters.Add(characterView);
            FieldView[pos.x, pos.y].Content = characterView;
        }
        
        private void OnCharacterAnimationStarted()
        {
            _prevState = State;
            State = BattleState.Animation;
        }
        private void OnCharacterAnimationFinished()
        {
            State = _prevState;
            GetCharacterView(CurrentCharacter).Highlight(HighlightType.CurrentTurn);
        }

        private void OnCharacterDied(Character character)
        {
            StartCoroutine(GetCharacterView(character).PlayAnimation(BattleAnimation.Death));
        }

        private void OnBattleStarted()
        {
            Debug.Assert(State == BattleState.Pause);
            State = BattleState.StartingBattle;
            Debug.Log("Battle started");
        }
        private void OnRoundStarted(int round)
        {
            Debug.Assert(State is BattleState.StartingBattle or BattleState.FinishingRound);
            State = BattleState.StartingRound;
            Debug.Log("Round started");
        }
        private void OnCharacterTurnStarted(Character character)
        {
            Debug.Assert(State is BattleState.StartingRound or BattleState.PlayerTurn or BattleState.EnemyTurn);
            State = character.Team == Team.Player ? BattleState.PlayerTurn : BattleState.EnemyTurn;
            CurrentCharacter = character;
            GetCharacterView(CurrentCharacter).Highlight(HighlightType.CurrentTurn);

            if (State == BattleState.EnemyTurn)
                StartCoroutine(ShowEnemyTurn());
            Debug.Log(character.Data.Name + "'s turn started");
        }

        private IEnumerator ShowEnemyTurn()
        {
            Controller.Lock();
            yield return new WaitForSeconds(1);
            Controller.Unlock();
        }
        
        private void OnCharacterTurnFinished(Character character)
        {
            GetCharacterView(CurrentCharacter).StopHighlight();
            CurrentCharacter = null;
            Debug.Log(character.Data.Name + " with id " + character.Id + " finished his turn");
        }
        private void OnRoundFinished()
        {
            Debug.Assert(State is BattleState.PlayerTurn or BattleState.EnemyTurn);
            State = BattleState.FinishingRound;
            Debug.Log("Round finished");
        }
        private void OnBattleFinished(Team winner)
        {
            //Debug.Assert(State == ); ???
            State = BattleState.FinishingBattle;
            Debug.Log("Battle finished");
            Debug.Log(winner + " team have won the battle");
        }

        public void EndTurn()
        {
            Controller.EndTurn();
        }

        private CharacterView GetCharacterView(Character character)
        {
            foreach (var view in _characters)
            {
                if (view.Character == character)
                    return view;
            }

            throw new ArgumentException("Cannot find CharacterView for " + character.Data.Name);
        }

        public void ResetState() => _state = _prevState;
    }

    public enum BattleState
    {
        Pause = 0,
        StartingBattle,
        StartingRound,
        PlayerTurn,
        EnemyTurn,
        Animation,
        TargetSelection,
        FinishingRound,
        FinishingBattle
    }
}