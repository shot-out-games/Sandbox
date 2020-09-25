using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SlashSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref SlashComponent slashComponent, in InputControllerComponent input) =>
        {

            if (slashComponent.slashActive == false) return;
            if (input.buttonA_Pressed == true && slashComponent.slashState == (int)SlashStates.None)
            {
                slashComponent.slashState = (int)SlashStates.Started;
                if (slashComponent.animate == false)
                {
                    slashComponent.animate = true;
                }
            }

        }).Schedule();

        Entities.WithoutBurst().ForEach((Animator animator, ref SlashComponent slashComponent) =>
        {
            if (slashComponent.slashActive == false) return;
            //animator.SetInteger("SlashState", 0);
            //slashComponent.slashState = (int) SlashStates.None;
            if (slashComponent.animate == true && animator.GetInteger("SlashState") == 0)
            {
                animator.SetInteger("SlashState", 1);
                Debug.Log("slash");
                slashComponent.animate = false;
            }
            else if (slashComponent.animate == false && animator.GetInteger("SlashState") == 0)//0 set by slashstate script in animator
            {
                slashComponent.slashState = (int) SlashStates.None;
            }


        }
        ).Run();


    }
}
