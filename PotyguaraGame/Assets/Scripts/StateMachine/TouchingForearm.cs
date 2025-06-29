using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingForearm : StateMachineBehaviour
{
    public int loop;
    private int count = 0;
    private int lastLoop = -1;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int currentLoop = Mathf.FloorToInt(stateInfo.normalizedTime);

        if (currentLoop > lastLoop)
        {
            lastLoop = currentLoop;
            count++;

            if (count >= loop)
            {
                animator.SetTrigger("Touching");
                count = 0;
            }
        }
    }
}