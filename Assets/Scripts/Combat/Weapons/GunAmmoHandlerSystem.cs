using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


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
                in AttachWeaponComponent attachWeapon) =>
            {


                if (attachWeapon.attachedWeaponSlot < 0 ||
                    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
                    )
                {
                    gun.Duration = 0;
                    gun.WasFiring = 0;
                    return;
                }


                if (dead.isDead) return;
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
                float rate = ammoDataComponent.Rate;
                float strength = ammoDataComponent.Strength;
                float damage = ammoDataComponent.Damage;


                gun.Duration += dt;
                if ((gun.Duration > rate) && (gun.IsFiring == 1))
                {

                    if (gun.PrimaryAmmo != null)
                    {
                        gun.IsFiring = 0;
                        statsComponent.shotsFired += 1;
                        Entity e = EntityManager.Instantiate(gun.PrimaryAmmo);
                        Translation translation = new Translation() { Value = bulletManager.AmmoStartLocation.position };
                        Rotation rotation = new Rotation() { Value = gun.AmmoStartRotation.Value };
                        //var playerVelocity = GetComponent<PhysicsVelocity>(entity);
                        var velocity = EntityManager.GetComponentData<PhysicsVelocity>(e);
                        float3 forward = bulletManager.AmmoStartLocation.forward;
                        //velocity.Linear = forward * (strength + math.abs(playerVelocity.Linear.x));
                        velocity.Linear = forward * (strength);

                        EntityManager.SetComponentData(e, translation);
                        EntityManager.SetComponentData(e, rotation);
                        EntityManager.SetComponentData(e, velocity);
                        var ammoComponent = EntityManager.GetComponentData<AmmoComponent>(e);
                        ammoComponent.OwnerAmmoEntity = entity;
                        EntityManager.SetComponentData(e, ammoComponent);
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

        //ecb.Playback(EntityManager);
        //ecb.Dispose();


    }



}
