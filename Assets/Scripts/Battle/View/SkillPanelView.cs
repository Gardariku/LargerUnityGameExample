using Battle.Data.Skills;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Battle.View
{
    public class SkillPanelView : MonoBehaviour
    {
        [SerializeField] private SkillData _defaultAttackSkill;
        [SerializeField] private SkillData _defaultMoveSkill;
        private BattleView _battleView;
        [SerializeField] private Button[] skills;

        [Inject]
        public void Init(BattleView battleView)
        {
            _battleView = battleView;
        }

        public void UseDefaultAttackSkill()
        {
            if (_battleView.State == BattleState.PlayerTurn)
                _battleView.Controller.TryUseSkill(_battleView.CurrentCharacter, _defaultAttackSkill);
        } 
        public void UseDefaultMoveSkill()
        {
            if (_battleView.State == BattleState.PlayerTurn)
                _battleView.Controller.TryUseSkill(_battleView.CurrentCharacter, _defaultMoveSkill);
        }
    }
}
