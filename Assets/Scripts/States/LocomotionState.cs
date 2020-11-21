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
    //    if (animationType == AnimationType.Aim)
    //    {
    //        animator.SetInteger("WeaponRaised", (int)WeaponMotion.None);
    //    }

    //}
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationType == AnimationType.Aim)
        {
            //animator.SetBool("Aim", false);
            if (animator.GetInteger("WeaponRaised") == (int) WeaponMotion.Started)
            {
                animator.SetInteger("WeaponRaised", (int)WeaponMotion.Raised);
            }

            Debug.Log("event aim");
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //// Implement code that processes and affects root motion
        //Vector3 velocity = animator.deltaPosition / Time.deltaTime * (float)animator.GetComponent<PlayerMove>().currentSpeed;
        //velocity.y = animator.GetComponent<Rigidbody>().velocity.y;//pass y from rigibody since rigidbody when on controls y force 
        //animator.GetComponent<Rigidbody>().velocity = velocity;

    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

}