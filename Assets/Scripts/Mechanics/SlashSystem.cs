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

        NativeArray<bool> hk = new NativeArray<bool>(1, Allocator.TempJob);
        //var slashGroup = GetComponentDataFromEntity<SlashComponent>(false);

        int currentLevel = 0;
        Entities.ForEach((in SkillTreeComponent skillTree) =>
        {
            currentLevel = skillTree.CurrentLevel;

        }
        ).Run();



        var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();

        bool special = false;

        Entities.WithoutBurst().ForEach((WeaponItem weaponItem, in Entity e) =>
        {

            if (bufferFromEntity.HasComponent(e))
            {
                var weaponItemComponent = bufferFromEntity[e];


                if (currentLevel >= 8 && weaponItemComponent[0].special == true && weaponItemComponent[0].pickedUp == true)
                {
                    weaponItem.pickedUp = true;
                    special = true;
                    Debug.Log("active");
                }
                else if(currentLevel >= 8 && weaponItemComponent[0].special == true && weaponItem.reset == false)
                {
                    weaponItem.reset = true;
                }

            }
        }


        ).Run();




        Entities.ForEach((ref SlashComponent slashComponent, in InputControllerComponent input, in Entity e) =>
        {

            if (slashComponent.slashActive == false) return;
            if ((input.leftTriggerPressed == true  || input.rightTriggerPressed == true && special == false) && slashComponent.slashState == (int)SlashStates.None)//why are triggers backward?
            {
                slashComponent.slashState = (int)SlashStates.Started;
                if (slashComponent.animate == false)
                {
                    slashComponent.animate = true;
                }

            }



            //if (input.buttonA_Pressed == true && slashComponent.slashState == (int)SlashStates.None) 
            if (input.rightTriggerPressed == true && special == true && slashComponent.slashState == (int)SlashStates.None)
            {
                slashComponent.slashState = (int)SlashStates.Started;
                if (slashComponent.animate == false)
                {
                    hk[0] = true;//not supported
                    //slashComponent.hkDamage += 1;
                    slashComponent.animate = true;
                }
            }

        }).Schedule();
        //}).Run();

        Entities.WithoutBurst().ForEach((Animator animator, ref SlashComponent slashComponent, ref WinnerComponent winner, in SkillTreeComponent skillTree) =>
            {
                if (slashComponent.slashActive == false) return;
                //animator.SetInteger("SlashState", 0);
                //slashComponent.slashState = (int) SlashStates.None;
                if (slashComponent.animate == true && animator.GetInteger("SlashState") == 0)
                {
                    if (hk[0] == true && skillTree.CurrentLevel >= 8)
                    {
                        animator.SetInteger("SlashState", 2);
                        slashComponent.hkDamage += 1;
                        if (slashComponent.hkDamage == 3)
                        {
                            animator.SetInteger("Dead", 1);
                            winner.winConditionMet = toggleStates.on;
                        }
                        hk[0] = false;

                    }
                    else if (hk[0] == true)
                    {
                        animator.SetInteger("SlashState", 2);
                        hk[0] = false;
                    }
                    else
                    {
                        animator.SetInteger("SlashState", 1);
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
