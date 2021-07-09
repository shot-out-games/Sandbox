using SandBox.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
//[UpdateInGroup(typeof(TransformSystemGroup))]

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class GunAmmoHandlerSystem : SystemBase
{

    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {

        if (LevelManager.instance.endGame == true) return;

        //EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);


        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration

        //Entities.WithoutBurst().ForEach((Entity e, BulletManager bulletManager) =>
        //{


        //}).Run();


        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();


        // Update the target position (must pass isReadOnly=false)
        //var gun = GetComponentDataFromEntity<GunComponent>(false);



        // Schedule the Entities.ForEach lambda job that will add Instantiate commands to the EntityCommandBuffer.
        // Since this job only runs on the first frame, we want to ensure Burst compiles it before running to get the best performance (3rd parameter of WithBurst)
        // The actual job will be cached once it is compiled (it will only get Burst compiled once).

        Entities.WithBurst(FloatMode.Default, FloatPrecision.Standard, true).WithNone<Pause>().ForEach(
            (
                 //BulletManager bulletManager,
                 Entity entity,
                 int entityInQueryIndex,


                in DeadComponent dead,
                in AttachWeaponComponent attachWeapon,
                 in ActorWeaponAimComponent actorWeaponAimComponent
                 ) =>
            {


                if (!HasComponent<GunComponent>(entity)) return;
                var gun = GetComponent<GunComponent>(entity);

                //var e = commandBuffer.Instantiate(entityInQueryIndex, gun.PrimaryAmmo);



                if (attachWeapon.attachedWeaponSlot < 0 ||
                    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                    )
                {
                    gun.Duration = 0;

                    gun.IsFiring = 0;
                    return;
                }


                //if (dead.isDead) return;
                bool isEnemy = HasComponent<EnemyComponent>(entity);

                if (isEnemy)
                {
                    if (HasComponent<EnemyWeaponMovementComponent>(entity) == false)
                    {
                        return;
                    }
                }

                Entity primaryAmmoEntity = gun.PrimaryAmmo;
                var ammoDataComponent = GetComponent<AmmoDataComponent>(primaryAmmoEntity);
                float rate = ammoDataComponent.GameRate;
                float strength = ammoDataComponent.GameStrength;
                float damage = ammoDataComponent.GameDamage;
                //change based on game
                if (gun.ChangeAmmoStats > 0)
                {
                    strength = strength * (100 - gun.ChangeAmmoStats * 2) / 100;
                    if (strength <= 0) strength = 0;
                }

                //var e = commandBuffer.Instantiate(entityInQueryIndex, gun.PrimaryAmmo);

                ////Translation translation = new Translation() { Value = bulletManager.AmmoStartLocation.position };//use bone mb transform




                gun.Duration += dt;
                if ((gun.Duration > rate) && (gun.IsFiring == 1))
                {

                    gun.Duration = 0;
                    gun.IsFiring = 0;

                    if (gun.PrimaryAmmo != null &&
                        (actorWeaponAimComponent.weaponRaised == WeaponMotion.Raised || isEnemy))
                    {
                        //Debug.Log("bullet " + entity);


                        gun.IsFiring = 0;
                        //statsComponent.shotsFired += 1;
                        var e = commandBuffer.Instantiate(entityInQueryIndex, gun.PrimaryAmmo);


                        ////float3 forward = bulletManager.AmmoStartLocation.transform.forward;
                        //float3 forward = gun.AmmoStartPosition.Value;
                        ////float3 forward = Vector3.forward;


                        //if (actorWeaponAimComponent.weaponCamera == CameraType.TopDown)
                        //{
                        //    velocity.Linear = forward * strength + playerVelocity.Linear;
                        //}
                        //else
                        //{
                        //    velocity.Linear = forward * strength;
                        //}


                        //var translation = new Translation() { Value = gun.AmmoStartPosition.Value };//use bone mb transform
                        //Rotation rotation = new Rotation() { Value = gun.AmmoStartRotation.Value };
                        //var playerVelocity = GetComponent<PhysicsVelocity>(entity);
                        //var velocity = GetComponent<PhysicsVelocity>(e);

                        var translation = new Translation() { Value = gun.AmmoStartPosition.Value };//use bone mb transform
                                                                                                    //Rotation rotation = new Rotation() { Value = gun.AmmoStartRotation.Value };
                                                                                                    //var playerVelocity = GetComponent<PhysicsVelocity>(entity);
                        //var velocity = GetComponent<PhysicsVelocity>(e);



                        //var ammoComponent = GetComponent<AmmoComponent>(e);
                        //ammoComponent.OwnerAmmoEntity = entity;//may not need use trigger instead
                        //var triggerComponent = GetComponent<TriggerComponent>(e);
                        //triggerComponent.ParentEntity = entity;
                        //commandBuffer.SetComponent(entityInQueryIndex, e, ammoComponent);
                        //commandBuffer.SetComponent(entityInQueryIndex, e, triggerComponent);

                        //if (bulletManager.weaponAudioClip && bulletManager.weaponAudioSource)
                        //{
                        //  bulletManager.weaponAudioSource.PlayOneShot(bulletManager.weaponAudioClip, .25f);
                        //}

                        gun.Duration = 0;





                        //if (EntityManager.HasComponent<Animator>(entity))
                        //{
                        //  bulletManager.GetComponent<Animator>().SetLayerWeight(0, 0);
                        //}


                        //commandBuffer.SetComponent(entityInQueryIndex, entity, translation);
                        //commandBuffer.SetComponent(entityInQueryIndex, e, rotation);
                        //commandBuffer.SetComponent(entityInQueryIndex, entity, velocity);


                    }
                }

                commandBuffer.SetComponent(entityInQueryIndex, entity, gun);



            }
        ).ScheduleParallel();


    

        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        //commandBuffer.Playback(EntityManager);
        //commandBuffer.Dispose();
        //ecb.Playback(EntityManager);
        //ecb.Dispose();


    }



}
