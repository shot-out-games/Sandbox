using SandBox.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


//[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateInGroup(typeof(TransformSystemGroup))]

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class GunAmmoHandlerSystem : SystemBase
{

    
    protected override void OnUpdate()
    {

        //EntityCommandBufferSystem barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        //var ecb = new EntityCommandBuffer(Allocator.Temp);
        //var ecb = barrier.CreateCommandBuffer().AsParallelWriter();
        //var ecb = barrier.CreateCommandBuffer();
        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration

        Entities.WithoutBurst().WithStructuralChanges().WithNone<Pause>().ForEach(
            (
                 BulletManager bulletManager,
                ref GunComponent gun, ref StatsComponent statsComponent,
                in RatingsComponent ratingsComponent,
                in Entity entity, in DeadComponent dead,
                in AttachWeaponComponent attachWeapon,
                 in ActorWeaponAimComponent actorWeaponAimComponent
                 ) =>
            {


                if (attachWeapon.attachedWeaponSlot < 0 ||
                    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                    )
                {
                    gun.Duration = 0;
                    return;
                }

                if(LevelManager.instance.endGame == true) return;

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
                float rate = ammoDataComponent.GameRate;
                float strength = ammoDataComponent.GameStrength;
                float damage = ammoDataComponent.GameDamage;
                //change based on game
                if (gun.ChangeAmmoStats > 0)
                {
                    strength = strength *(100- gun.ChangeAmmoStats * 2) / 100;
                    if (strength <= 0) strength = 0;
                }


                gun.Duration += dt;
                if ((gun.Duration > rate) && (gun.IsFiring == 1))
                {

                    if (gun.PrimaryAmmo != null &&
                        (actorWeaponAimComponent.weaponRaised == WeaponMotion.Raised || isEnemy))
                    {
                        gun.IsFiring = 0;
                        statsComponent.shotsFired += 1;
                        //gun.gameDamage = strength;//current gun strength used by attacker handler collision system
                        Entity e = EntityManager.Instantiate(gun.PrimaryAmmo);
                        Translation translation = new Translation() { Value = bulletManager.AmmoStartLocation.position };//use bone mb transform
                        Rotation rotation = new Rotation() { Value = gun.AmmoStartRotation.Value };
                        var playerVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
                        var velocity = EntityManager.GetComponentData<PhysicsVelocity>(e);
                        float3 forward = bulletManager.AmmoStartLocation.transform.forward;
                        if (actorWeaponAimComponent.weaponCamera == CameraType.TopDown)
                        {
                            velocity.Linear = forward * strength + playerVelocity.Linear;
                        }
                        else
                        {
                            velocity.Linear = forward * strength;
                        }



                        //Matrix4x4 matrix4x4 = Matrix4x4.identity;
                        //matrix4x4.SetTRS(translation.Value, rotation.Value, Vector3.one);
                        //Vector3 localDirection = matrix4x4.inverse.MultiplyVector(forward);
                        //Vector3 worldDirection = matrix4x4.MultiplyVector(forward);
                        //velocity.Linear = worldDirection * strength;

                        //if (playerWeaponAimComponent.weapon2d) velocity.Linear.z = 0;

                        EntityManager.SetComponentData(e, translation);
                        EntityManager.SetComponentData(e, rotation);
                        EntityManager.SetComponentData(e, velocity);
                        var ammoComponent = EntityManager.GetComponentData<AmmoComponent>(e);
                        ammoComponent.OwnerAmmoEntity = entity;//may not need use trigger instead
                        var triggerComponent = EntityManager.GetComponentData<TriggerComponent>(e);
                        triggerComponent.ParentEntity = entity;
                        EntityManager.SetComponentData(e, ammoComponent);
                        EntityManager.SetComponentData(e, triggerComponent);

                        if (bulletManager.weaponAudioClip && bulletManager.weaponAudioSource)
                        {
                            bulletManager.weaponAudioSource.PlayOneShot(bulletManager.weaponAudioClip);
                        }
                        gun.Duration = 0;





                        if (EntityManager.HasComponent<Animator>(entity))
                        {
                            bulletManager.GetComponent<Animator>().SetLayerWeight(0, 0);
                        }



                    }
                }
            }
        ).Run();

        //ecb.Playback(EntityManager);
        //ecb.Dispose();


    }



}
