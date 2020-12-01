using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;




public class LocomotionState : StateMachineBehaviour
{
    public AnimationType animationType;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{


    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{




    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
     
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("loco exit");

        if (animationType == AnimationType.Aim)
        {
            animator.SetInteger("WeaponRaised", (int)WeaponMotion.Raised);
            //Debug.Log("event aim");
        }

        if (animationType == AnimationType.Lowering)
        {
            animator.SetInteger("WeaponRaised", (int)WeaponMotion.None);
            //Debug.Log("event lowered ");
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

      

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

}