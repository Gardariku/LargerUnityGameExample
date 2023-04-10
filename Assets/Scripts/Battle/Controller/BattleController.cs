using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Controller.Commands;
using Common.Data_Structures;
using Battle.Model;
using Battle.Model.Characters;
using Battle.Model.Skills;
using UnityEngine;

namespace Battle.Controller
{
    public class BattleController : MonoBehaviour
    {
        public BattleModel BattleModel => BattleLoop.Model;
        public RoundModel RoundModel => RoundLoop.Model;
        public TurnModel TurnModel => TurnLoop.Model;
        // Commands added to this queue will be executed AFTER execution of command, that added them
        private Deque<IBattleCommand> _mainQueue = new ();
        // Commands added to this queue will be executed DURING execution of command, that added them
        private Deque<IBattleCommand> _batchQueue = new ();
        // This stack represents general game flow rules and probably shouldn't be changed mid-game
        private Stack<IEnumerator> _gameLoops = new ();
        [SerializeField] private int _locks;
        public bool IsLocked => _locks > 0;
        private bool _isBusy = true;
        // TODO: Work this around
        public bool DontAcceptSubcommands;

        public GameStateEventsContainer GameStateEvents = new GameStateEventsContainer();
        public CharacterEventsContainer CharacterEvents = new CharacterEventsContainer();

        public BattleLoop BattleLoop { get; private set; }
        public RoundLoop RoundLoop { get; private set; }
        public TurnLoop TurnLoop { get; private set; }

        public void Setup(FighterData[] PlayerTeam, FighterData[] EnemyTeam)
        {
            BattleLoop = new BattleLoop(this);
            RoundLoop = new RoundLoop(this);
            TurnLoop = new TurnLoop(this);

            BattleLoop.Model = new BattleModel(PlayerTeam, EnemyTeam, this);
            AddLoop(BattleLoop.Start());
            _isBusy = false;
        }

        private void Update()
        {
            if (_batchQueue.Count > 0)
                _batchQueue.RemoveFirst().Execute(this, BattleModel);
            
            if (_locks > 0 || _isBusy) return;
            if (_mainQueue.Count > 0)
            {
                _isBusy = true;
                _mainQueue.RemoveFirst().Execute(this, BattleModel);
                _isBusy = false;
            }
            else if (_gameLoops.Count > 0)
            {
                _isBusy = true;
                try
                {
                    IEnumerator execution = _gameLoops.Peek();
                    if (!execution.MoveNext()) _gameLoops.Pop();
                    var nextExecution = execution.Current as IEnumerator;
                    if (!(nextExecution is null)) _gameLoops.Push(nextExecution);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    _gameLoops.Pop();
                }
                _isBusy = false;
            }
        }

        #region QueueOperations

        public void AddLoop(IEnumerator loop)
        {
            _gameLoops.Push(loop);
            Update();
        }
        
        public void AddCommandMainFirst(params IBattleCommand[] commands)
        {
            _mainQueue.AddFirst(commands);
            Update();
        }
        public void AddCommandMainFirst(IEnumerable<IBattleCommand> commands)
        {
            _mainQueue.AddFirst(commands);
            Update();
        }
        public void AddCommandMainLast(params IBattleCommand[] commands)
        {
            _mainQueue.AddLast(commands);
            Update();
        }
        public void AddCommandMainLast(IEnumerable<IBattleCommand> commands)
        {
            _mainQueue.AddLast(commands);
            Update();
        }
        
        public void AddCommandBatchFirst(params IBattleCommand[] commands)
        {
            _batchQueue.AddFirst(commands);
            Update();
        }
        public void AddCommandBatchFirst(IEnumerable<IBattleCommand> commands)
        {
            _batchQueue.AddFirst(commands);
            Update();
        }
        public void AddCommandBatchLast(params IBattleCommand[] commands)
        {
            _batchQueue.AddLast(commands);
            Update();
        }
        public void AddCommandBatchLast(IEnumerable<IBattleCommand> commands)
        {
            _batchQueue.AddLast(commands);
            Update();
        }

        public void ClearQueueMain()
        {
            _mainQueue.Clear();
        }
        
        public void ClearQueueBatch()
        {
            _batchQueue.Clear();
        }

        #endregion

        public void Lock()
        {
            _locks++;
        }
        public void Unlock()
        {
            _locks--;
            Update();
        }

        public void EndTurn()
        {
            TurnLoop.EndTurn();
        }

        // TODO: Move these methods to Character
        public void UseSkill(Character user, SkillData skill, List<Character> targets)
        {
            _mainQueue.AddLast(new UseSkillCommand(skill, user, targets));
        }

        public bool TryAttack(Character attacker, Character target)
        {
            if (!(attacker.DiminishingStats.TryGetValue("STAMINA", out var stamina)
                  && stamina.CurrentValue > 0)) return false;
            if (!(attacker.CurrentStats.GetStatInt("RANGE") >= BattleModel.Field.CalculateDistance(attacker, target))) 
                return false;
            
            AddCommandMainFirst(new AttackCommand(attacker, target));
            AddCommandMainLast(new ChangeDimStatCommand(attacker, stamina, stamina.CurrentValue - 1));
            return true;
        }

        public bool TryMove(Character character, Vector2Int targetPosition)
        {
            Debug.Log($"Tried to move character {character.Data.Name} to {targetPosition}");
            Vector2Int[] path = BattleModel.Field.FindPath(character.Position, targetPosition);
            if (!(character.DiminishingStats.TryGetValue("STAMINA", out var stamina) && stamina.CurrentValue > 0 
                    && character.DiminishingStats.TryGetValue("SPEED", out var speed) 
                    && speed.CurrentValue + 0.05f > BattleModel.Field.CalculatePathLength(path)) || path.Length == 0)
                return false;
            
            AddCommandMainFirst(new ChangeDimStatCommand(character, stamina, stamina.CurrentValue - 1));
            AddCommandMainLast(new MoveCharacterCommand(character, path));
            return true;
        }

        public void CheckWinCondition(Character character)
        {
            if (BattleModel.Winner != Team.None)
                EndBattle();
        }
        
        public void EndBattle()
        {
            AddCommandMainFirst(new EndGameCommand());
        }

        public void CheatWin()
        {
            foreach (var enemy in BattleModel.Characters)
                enemy.DiminishingStats["HEALTH"].CurrentValue = 0;
            EndBattle();
        }

        public class GameStateEventsContainer
        {
            public Action GameStarted;
            public Action<Team> GameEnded;
            public Action<int> RoundStarted;
            public Action RoundEnded;
            public Action<Character> CharacterTurnStarted;
            public Action<Character> CharacterTurnEnded;
        }

        public class CharacterEventsContainer
        {
            public Action<MoveCharacterCommand> CharacterMoveStarted;
            public Action<MoveCharacterCommand> CharacterMoveFinished;
            public Action<StepCommand> CharacterStepStarted;
            public Action<StepCommand> CharacterStepFinished;
            public Action<AttackCommand> CharacterAttackStarted;
            public Action<AttackCommand> CharacterAttackFinished;
            public Action<UseSkillCommand> CharacterAbilityUsageStarted;
            public Action<UseSkillCommand> CharacterAbilityUsageFinished;
            public Action<Character> CharacterDeathStarted;
            public Action<Character> CharacterDeathFinished;
            public Action<Character> CharacterSummonStarted;
            public Action<Character> CharacterSummonFinished;
            public Action<DealDamageCommand> CharacterDamageDealingStarted;
            public Action<DealDamageCommand> CharacterDamageDealingFinished;
            public Action<RestoreHealthCommand> CharacterHealStarted;
            public Action<RestoreHealthCommand> CharacterHealFinished;
            public Action<ChangeDimStatCommand> CharacterDiminishingStatChanged;
        }
    }


}