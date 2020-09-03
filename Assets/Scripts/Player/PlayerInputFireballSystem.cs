using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

//[DisableAutoCreation]

public class PlayerInputFireballSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach
        (
            (
                ref FireballWeaponComponent handComponent,
                in Entity entity,
                in InputController inputController,
                in Animator animator,
                in AttachWeaponComponent attachWeapon
            ) =>
            {

                handComponent.IsFiring = 0;
                float lt_value = inputController.leftTriggerValue;
                float rt_value = inputController.rightTriggerValue;


                if (attachWeapon.attachedWeaponSlot >= 0 && attachWeapon.attachWeaponType == (int)WeaponType.Fireball && Math.Abs(lt_value) > .000001)
                {
                    animator.SetBool("Fireball", true);
                }

                if (attachWeapon.attachSecondaryWeaponType == (int)WeaponType.Fireball && Math.Abs(rt_value) > .000001)
                {
                    animator.SetBool("Fireball", true);
                }


            }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;

    }


    //public void SetAnimationLayerWeights(Animator animator)
    //{


    //        animator.SetLayerWeight(0, 0);
    //        animator.SetLayerWeight(1, 1); s weapon layer
    //        animator.SetBool("Aim", true);

    //}


}