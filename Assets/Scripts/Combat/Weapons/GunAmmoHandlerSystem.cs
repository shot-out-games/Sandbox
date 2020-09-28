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


public class GunAmmoHandlerSystem : JobComponentSystem
{



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration





        Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (ref GunComponent gun, ref LocalToWorld gunTransform, ref StatsComponent statsComponent, 
                ref Rotation gunRotation,
                in RatingsComponent ratingsComponent,
                in GunScript gunScript, in Entity entity, in AttachWeaponComponent attachWeapon) =>
            {


                Debug.Log("gun0 " + EntityManager.HasComponent<PlayerComponent>(entity));


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

                Debug.Log("gun1 " + EntityManager.HasComponent<PlayerComponent>(entity));


                if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;

                if (EntityManager.HasComponent<EnemyComponent>(entity))
                {
                    if(EntityManager.HasComponent<EnemyWeaponMovementComponent>(entity) == false)
                    {
                        return;
                    }
                }

                Debug.Log("gun2 " + EntityManager.HasComponent<PlayerComponent>(entity) + " firing " + gun.IsFiring);

                gun.Duration += dt;
                if ((gun.Duration > gun.gameRate) && (gun.IsFiring == 1))
                {
                    if (gun.Bullet != null)
                    {

                        Vector3 pos = gunScript.AmmoStartLocation.position;
                        Quaternion rot = gunScript.AmmoStartLocation.rotation;


                        gun.IsFiring = 0;
                        statsComponent.shotsFired += 1;
                        Entity e = EntityManager.Instantiate(gun.Bullet);
                        Translation translation = new Translation { Value = pos };
                        Rotation rotation = new Rotation { Value = rot };

                        var playerVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);

                        var velocity = EntityManager.GetComponentData<PhysicsVelocity>(e);
                        var mass = EntityManager.GetComponentData<PhysicsMass>(e);
                        float3 forward = gunScript.AmmoStartLocation.forward;
                        velocity.Linear = forward * (gun.gameStrength + math.abs(playerVelocity.Linear.x));
                        //Debug.Log("gun " + gun.gameStrength);
                        EntityManager.SetComponentData(e, translation);
                        EntityManager.SetComponentData(e, rotation);
                        EntityManager.SetComponentData(e, velocity);
                        gunScript.CreateBulletInstance(e);

                    }
                    gun.Duration = 0;
                }
                gun.WasFiring = 1;
            }
        ).Run();





        return default;
    }



}
