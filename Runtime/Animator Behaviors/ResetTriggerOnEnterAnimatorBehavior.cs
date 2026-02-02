using UnityEngine;

public class ResetTriggerOnEnterAnimatorBehavior : StateMachineBehaviour
{
    [Tooltip("leave blank to clear *every* trigger")]
    public string triggerToReset;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (string.IsNullOrEmpty(triggerToReset))
        {
            // nuke all triggers
            foreach (var p in animator.parameters)
                if (p.type == AnimatorControllerParameterType.Trigger)
                    animator.ResetTrigger(p.name);
        }
        else
        {
            animator.ResetTrigger(triggerToReset);
        }
    }
}