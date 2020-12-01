using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering.Universal;
using UnityEngine;


[UpdateBefore(typeof(GunAmmoHandlerSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateInGroup(typeof(TransformSystemGroup))]



public class PlayerInputAmmoSystem : SystemBase
{

    protected override void OnUpdate()
    {

        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        //bool lt_released = false;

        Entities.WithoutBurst().ForEach
        (
            (
                Animator animator,
                ref GunComponent gunComponent,
                ref PlayerWeaponAimComponent playerWeaponAimComponent,
                in Entity entity,
                in InputControllerComponent inputController,
                    in AttachWeaponComponent attachWeapon
            ) =>
            {
                //lt mapped to 1 on keyboard when LT is not used for shooting - if not map to left mouse
                float dpadY = inputController.dpadY;
                WeaponMotion currentWeaponMotion =  (WeaponMotion)animator.GetInteger("WeaponRaised");
                playerWeaponAimComponent.weaponRaised = currentWeaponMotion;
                bool ltPressed = inputController.leftTriggerPressed;

                if (
                    attachWeapon.attachWeaponType == (int)WeaponType.Gun && ltPressed == true ||
                attachWeapon.attachSecondaryWeaponType == (int)WeaponType.Gun && ltPressed == true
                    )
                {

                    gunComponent.IsFiring = 1;
                    playerWeaponAimComponent.weaponUpTimer = 0;
                    if (playerWeaponAimComponent.weaponRaised == WeaponMotion.None)
                    {
                        playerWeaponAimComponent.weaponRaised = WeaponMotion.Started;
                        //if not currently raised up then start
                        SetAnimationLayerWeights(animator, WeaponMotion.Started);
                    }

                }

                SetComponent(entity, gunComponent);

                if (dpadY > .000001 && playerWeaponAimComponent.weaponRaised == WeaponMotion.None)
                {
                    playerWeaponAimComponent.weaponRaised = WeaponMotion.Started;
                    SetAnimationLayerWeights(animator, WeaponMotion.Started);
                }
                else if (playerWeaponAimComponent.weaponRaised == WeaponMotion.None)
                {
                    SetAnimationLayerWeights(animator, WeaponMotion.None);
                }
                else if (playerWeaponAimComponent.weaponRaised == WeaponMotion.Raised)
                {
                    playerWeaponAimComponent.weaponUpTimer += Time.DeltaTime;
                    if (playerWeaponAimComponent.weaponUpTimer > 2)
                    {
                        playerWeaponAimComponent.weaponUpTimer = 0;
                        playerWeaponAimComponent.weaponRaised = WeaponMotion.Lowering;
                        SetAnimationLayerWeights(animator, WeaponMotion.Lowering);

                    }
                }

            }
        ).Run();


    }


    public void SetAnimationLayerWeights(Animator animator, WeaponMotion weaponMotion)
    {

        if (weaponMotion == WeaponMotion.Started)
        {
            animator.SetInteger("WeaponRaised", 1);
            animator.SetLayerWeight(0, 0);
            animator.SetLayerWeight(2, 1);

        }
        else if (weaponMotion == WeaponMotion.None)
        {
            animator.SetInteger("WeaponRaised", 0);
            animator.SetLayerWeight(0, 1);
            animator.SetLayerWeight(2, 0);

        }
        else if (weaponMotion == WeaponMotion.Lowering)
        {
            animator.SetInteger("WeaponRaised", 3);
        }
    }


}