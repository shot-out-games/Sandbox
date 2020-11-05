using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]

//[UpdateBefore(typeof(NdeMechanicSystem))]


public class GunAmmoHandlerSystem : SystemBase
{



    protected override void OnUpdate()
    {


        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration





        Entities.WithoutBurst().WithStructuralChanges().WithNone<Pause>().ForEach(
            (ref GunComponent gun, ref StatsComponent statsComponent,
                ref Rotation gunRotation,
                in RatingsComponent ratingsComponent,
                in BulletManager bulletManager,
                in Entity entity, in DeadComponent dead,
                in AttachWeaponComponent attachWeapon) =>
            {


                if (attachWeapon.attachedWeaponSlot < 0 ||
                    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                    )
                {
                    //gun.IsFiring = 0;
                    gun.Duration = 0;
                    gun.WasFiring = 0;
                    return;
                }


                //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (dead.isDead) return;
                bool isEnemy = EntityManager.HasComponent<EnemyComponent>(entity);

                if (isEnemy)
                {
                    if (EntityManager.HasComponent<EnemyWeaponMovementComponent>(entity) == false)
                    {
                        return;
                    }
                }


                Entity primaryAmmoEntity = gun.PrimaryAmmo;
                var ammoDataComponent = EntityManager.GetComponentData<AmmoDataComponent>(primaryAmmoEntity);
                float rate = ammoDataComponent.Rate;
                float strength = ammoDataComponent.Strength;
                float damage = ammoDataComponent.Damage;




                gun.Duration += dt;
                if ((gun.Duration > rate) && (gun.IsFiring == 1))
                //    if ((gun.Duration > gun.gameRate) && (gun.IsFiring == 1))
                {
                    //Debug.Log("gun " + gun.PrimaryAmmo);


                    if (gun.PrimaryAmmo != null)
                    {

                        //Debug.Log("gun2 " + EntityManager.HasComponent<PlayerComponent>(entity) + " firing " + gun.IsFiring);


                        gun.IsFiring = 0;
                        statsComponent.shotsFired += 1;


                        //Debug.Log("inst ");

                        Entity e = EntityManager.Instantiate(gun.PrimaryAmmo);
                        //Translation translation = new Translation { Value = pos };
                        //Rotation rotation = new Rotation { Value = rot };

                        Translation translation = new Translation() { Value = bulletManager.AmmoStartLocation.position };
                        Rotation rotation = new Rotation() { Value = gun.AmmoStartRotation.Value };


                        var playerVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);

                        var velocity = EntityManager.GetComponentData<PhysicsVelocity>(e);
                        var mass = EntityManager.GetComponentData<PhysicsMass>(e);
                        float3 forward = bulletManager.AmmoStartLocation.forward;
                        //float3 forward = gun.AmmoStartPosition.Value * math.forward();
                        //velocity.Linear = forward * (gun.gameStrength + math.abs(playerVelocity.Linear.x));
                        velocity.Linear = forward * (strength + math.abs(playerVelocity.Linear.x));
                        //velocity.Linear = gun.gameStrength;


                        EntityManager.SetComponentData(e, translation);
                        EntityManager.SetComponentData(e, rotation);
                        EntityManager.SetComponentData(e, velocity);
                        var ammoComponent = EntityManager.GetComponentData<AmmoComponent>(e);
                        ammoComponent.OwnerAmmoEntity = entity;
                        EntityManager.SetComponentData(e, ammoComponent);
                        //bulletManager.CreatePrimaryAmmoInstance(e);
                        if (bulletManager.weaponAudioClip && bulletManager.weaponAudioSource)
                        {
                            bulletManager.weaponAudioSource.PlayOneShot(bulletManager.weaponAudioClip);
                        }

                    }
                    gun.Duration = 0;
                }
                gun.WasFiring = 1;
            }
        ).Run();




    }



}
