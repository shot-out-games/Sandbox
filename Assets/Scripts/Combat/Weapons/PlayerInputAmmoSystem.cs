using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
//[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateBefore(typeof(GunAmmoHandlerSystem))]



public class PlayerInputAmmoSystem : SystemBase
{

    protected override void OnUpdate()
    {

        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        //bool lt_released = false;

        var check = new NativeArray<int>(1, Allocator.TempJob);


        Entities.WithoutBurst().ForEach
        (
            (
                Animator animator,
                BulletManager bulletManager,
                ref GunComponent gunComponent,
                ref ActorWeaponAimComponent playerWeaponAimComponent,
                in Entity entity,
                in InputControllerComponent inputController,
                    in AttachWeaponComponent attachWeapon
            ) =>
            {
                //lt mapped to 1 on keyboard when LT is not used for shooting - if not map to left mouse
                float dpadY = inputController.dpadY;
                WeaponMotion currentWeaponMotion = (WeaponMotion)animator.GetInteger("WeaponRaised");
                playerWeaponAimComponent.weaponRaised = currentWeaponMotion;
                bool ltPressed = inputController.leftTriggerDown || inputController.leftTriggerPressed;

                if (
                    //gunComponent.Duration > 0 &&
                    (attachWeapon.attachWeaponType == (int)WeaponType.Gun && ltPressed == true ||
                        attachWeapon.attachSecondaryWeaponType == (int)WeaponType.Gun && ltPressed == true)
                    )
                {

                    gunComponent.IsFiring = 1;
                    //Debug.Log("firing " + entity);
                    playerWeaponAimComponent.weaponUpTimer = 0;
                    if (playerWeaponAimComponent.weaponRaised == WeaponMotion.None)
                    {
                        playerWeaponAimComponent.weaponRaised = WeaponMotion.Started;
                        //if not currently raised up then start
                        SetAnimationLayerWeights(animator, WeaponMotion.Started);
                    }

                    if (HasComponent<ScoreComponent>(entity))
                    {
                        var scoreComponent = GetComponent<ScoreComponent>(entity);
                        scoreComponent.lastShotConnected = false;
                        SetComponent<ScoreComponent>(entity, scoreComponent);

                    }




                    check[0] = 1;



                }

                //SetComponent(entity, gunComponent);

                //if (dpadY > .000001 && playerWeaponAimComponent.weaponRaised == WeaponMotion.None)
                //{
                //  playerWeaponAimComponent.weaponRaised = WeaponMotion.Started;
                //SetAnimationLayerWeights(animator, WeaponMotion.Started);
                //}
                if (playerWeaponAimComponent.weaponRaised == WeaponMotion.None)
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

        check.Dispose();
        //if (check[0] == 1) Debug.Log("done");


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