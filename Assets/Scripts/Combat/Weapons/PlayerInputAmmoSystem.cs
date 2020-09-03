using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


public class PlayerInputAmmoSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        //bool lt_released = false;

        Entities.WithoutBurst().ForEach
        (
            (
                ref GunComponent gunComponent,
                ref PlayerWeaponAimComponent playerWeaponAimComponent,
                in Entity entity,
                in InputController inputController,
                in Animator animator,
                    in AttachWeaponComponent attachWeapon
            ) =>
            {


                gunComponent.IsFiring = 0;
                float rt_value = inputController.rightTriggerValue;
                float dpadY = inputController.dpadY;


                bool ltPressed = inputController.leftTriggerDown;
                bool ltReleased = inputController.leftTriggerUp;

                bool rtPressed = inputController.rightTriggerDown;
                bool rtReleased = inputController.rightTriggerUp;





                if (attachWeapon.attachedWeaponSlot >= 0 &&
                    attachWeapon.attachWeaponType == (int)WeaponType.Gun
                    && ltReleased == true
                    )
                {
                    gunComponent.IsFiring = 1;
                    playerWeaponAimComponent.weaponUpTimer = 0;
                    playerWeaponAimComponent.weaponRaised = true;
                    SetAnimationLayerWeights(animator, true);
                }

                if (attachWeapon.attachSecondaryWeaponType == (int)WeaponType.Gun && rtPressed == true)
                {
                    gunComponent.IsFiring = 1;
                    playerWeaponAimComponent.weaponUpTimer = 0;
                    playerWeaponAimComponent.weaponRaised = true;
                    SetAnimationLayerWeights(animator, true);
                }


                ecb.SetComponent(entity, gunComponent);

                if (dpadY > .000001)
                {
                    playerWeaponAimComponent.weaponRaised = true;
                    SetAnimationLayerWeights(animator, true);
                }

                if (playerWeaponAimComponent.weaponRaised)
                {
                    playerWeaponAimComponent.weaponUpTimer += Time.DeltaTime;
                    if (playerWeaponAimComponent.weaponUpTimer > 2)
                    {
                        playerWeaponAimComponent.weaponUpTimer = 0;
                        playerWeaponAimComponent.weaponRaised = false;
                        SetAnimationLayerWeights(animator, false);
                    }
                }

            }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();

        return default;

    }


    public void SetAnimationLayerWeights(Animator animator, bool weaponRaised)
    {
        if (animator.GetComponent<PlayerWeaponAim>())
        {
            animator.GetComponent<PlayerWeaponAim>().weaponRaised = weaponRaised;
        }


        if (weaponRaised)
        {
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(1, 1); //1 is weapon layer
            animator.SetBool("Aim", true);

        }
        else if (!weaponRaised)
        {
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(1, 0);
            animator.SetBool("Aim", false);
        }
    }


}