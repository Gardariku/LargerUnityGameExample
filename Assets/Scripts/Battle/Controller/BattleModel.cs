using System;
using System.Collections.Generic;
using Battle.Controller.Field;
using Battle.Model;
using Battle.Model.Characters;
using UnityEngine;

namespace Battle.Controller
{
    [Serializable]
    public class BattleModel
    {
        [field: SerializeField] public int Round { get; set; }
        [field: SerializeField] public List<Character> Characters { get; private set; } = new ();
        [field: SerializeField] public BattleField Field { get; private set; }
        private int _nextId = 0;
        public List<Character> Allies
        {
            get
            {
                List<Character> allies = new List<Character>();
                foreach (var character in Characters)
                {
                    if (character.Team == Team.Player && character.IsAlive)
                        allies.Add(character);
                }

                return allies;
            }
        }
        public List<Character> Enemies
        {
            get
            {
                List<Character> enemies = new List<Character>();
                foreach (var character in Characters)
                {
                    if (character.Team == Team.Enemy && character.IsAlive)
                        enemies.Add(character);
                }

                return enemies;
            }
        }

        private BattleController _controller;

        public Team Winner
        {
            get
            {
                bool check = false;
                foreach (var ally in Allies)
                {
                    if (ally.IsAlive)
                        check = true;
                }
                if (!check)
                    return Team.Enemy;
                
                check = false;
                foreach (var enemy in Enemies)
                {
                    if (enemy.IsAlive)
                        check = true;
                }
                if (!check)
                    return Team.Player;

                return Team.None;
            }
        }

        public BattleModel(FighterData[] PlayerTeam, FighterData[] EnemyTeam, BattleController controller)
        {
            _controller = controller;
            Vector2Int fieldSize = new Vector2Int(10, 6);

            int i = 0;
            foreach (var characterData in PlayerTeam)
            {
                Character character = new Character(characterData, Team.Player, controller, _nextId++);
                Characters.Add(character);
                
                // TODO: choose/adjust default position
                character.Position = new Vector2Int(0, i);
                i++;
            }
            i = 0;
            foreach (var characterData in EnemyTeam)
            {
                Character character = new Character(characterData, Team.Enemy, controller, _nextId++);
                Characters.Add(character);
                
                character.Position = new Vector2Int(fieldSize.x - 1, i);
                i++;
            }
            
            Field = new BattleField(fieldSize, Characters, controller);
        }
    }
}