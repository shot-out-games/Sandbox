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
        //NativeArray<bool> hk = new NativeArray<bool>(1, Allocator.TempJob);



        //var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();





        Entities.ForEach((ref SlashComponent slashComponent, in InputControllerComponent input, in Entity e) =>
        {

            if (slashComponent.slashActive == false) return;
            slashComponent.slashState = (int)SlashStates.None;
            if ((input.buttonX_Pressed == true) && slashComponent.slashState == (int)SlashStates.None)//why are triggers backward?
            {
                slashComponent.slashState = (int)SlashStates.Started;
                if (slashComponent.animate == false)
                {
                    slashComponent.animate = true;
                }

            }

        }
        ).Run();


        Entities.WithoutBurst().WithStructuralChanges().ForEach((
            Animator animator, PlayerCombat playerCombat, WeaponManager weaponManager,
            ref SlashComponent slashComponent) =>
            {
                if (slashComponent.slashActive == false) return;
                if (weaponManager.primaryAttached == false)
                {
                    slashComponent.animate = false;
                    slashComponent.slashState = (int)SlashStates.None;
                    return;
                }

                if (slashComponent.animate == true && animator.GetInteger("SlashState") == 0)
                {
                    playerCombat.SelectMove(3);
                    Debug.Log("slash");
                    slashComponent.animate = false;
                }
                //else if (slashComponent.animate == false && animator.GetInteger("SlashState") == 0)//0 set by slashstate script in animator
                //{
                   // slashComponent.slashState = (int)SlashStates.None;
                    //Debug.Log("slash system end slash");
                    //animator.SetInteger("CombatAction", 0);
                //}
            }

            ).Run();




    }

        
}
