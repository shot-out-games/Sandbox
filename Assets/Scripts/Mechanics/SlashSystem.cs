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

        bool hk = false;

        Entities.ForEach((ref SlashComponent slashComponent, in InputControllerComponent input) =>
        {

            if (slashComponent.slashActive == false) return;
            if (input.rightTriggerPressed == true && slashComponent.slashState == (int)SlashStates.None)//why are triggers backward?
            {
                slashComponent.slashState = (int)SlashStates.Started;
                if (slashComponent.animate == false)
                {
                    hk = true;//not supported
                    slashComponent.hkDamage += 1;
                    slashComponent.animate = true;
                }

            }



            //if (input.buttonA_Pressed == true && slashComponent.slashState == (int)SlashStates.None) 
            if (input.leftTriggerPressed == true && slashComponent.slashState == (int)SlashStates.None)
            {
                slashComponent.slashState = (int)SlashStates.Started;
                if (slashComponent.animate == false)
                {
                    slashComponent.animate = true;
                }
            }

            //}).Schedule();
        }).Run();

        Entities.WithoutBurst().ForEach((Animator animator, ref SlashComponent slashComponent) =>
            {
                if (slashComponent.slashActive == false) return;
            //animator.SetInteger("SlashState", 0);
            //slashComponent.slashState = (int) SlashStates.None;
            if (slashComponent.animate == true && animator.GetInteger("SlashState") == 0)
                {
                    if (hk == true && slashComponent.hkDamage == 3)
                    {
                        animator.SetInteger("SlashState", 1);
                       slashComponent.hkDamage += 1;
                       hk = false;

                    }
                    else if (hk == true)
                    {
                        animator.SetInteger("SlashState", 1);
                        slashComponent.hkDamage += 1;
                        hk = false;
                    }
                    else
                    {
                        animator.SetInteger("SlashState", 2);
                    }

                    Debug.Log("slash");
                    slashComponent.animate = false;
                }
                else if (slashComponent.animate == false && animator.GetInteger("SlashState") == 0)//0 set by slashstate script in animator
                {
                    slashComponent.slashState = (int)SlashStates.None;
                }


            }
            ).Run();


    }
}
