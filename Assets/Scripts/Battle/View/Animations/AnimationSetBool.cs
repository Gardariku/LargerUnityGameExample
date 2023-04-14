using UnityEngine;

namespace Battle.View.Animations
{
    public class AnimationSetBool : StateMachineBehaviour
    {
        public bool Value;
        public string BoolName;
    
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(BoolName, Value);
        }
    }
}
