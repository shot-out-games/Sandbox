using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;


//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class GunAmmoHandlerSystem : JobComponentSystem
{



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration





        Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (ref GunComponent gun, ref LocalToWorld gunTransform, ref StatsComponent statsComponent,
                ref Rotation gunRotation, in GunScript gunScript, in Entity entity, in AttachWeaponComponent attachWeapon) =>
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



                if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;


                gun.Duration += dt;
                //if ((gun.Duration > gun.Rate) || (gun.WasFiring == 0))
                if ((gun.Duration > gun.Rate) && (gun.IsFiring == 1))
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

                        //ComponentExtensions.ApplyImpulse(ref velocity, new PhysicsMass(), position , rotation,  gunScript.Strength, position.Value);




                        //float3 direction = math.normalizesafe(gunScript.AmmoStartLocation.forward * velocity.Linear);
                        //float3  translate = math.mul(rotation.Value, gunScript.AmmoStartLocation.forward)  ;



                        //float3 forward = rot * pos;
                        float3 forward = gunScript.AmmoStartLocation.forward;


                        //Debug.Log("fwd " + forward);

                        //forward = gunScript.AmmoStartLocation.transform.TransformPoint(forward);

                        //Debug.Log("fwd1 " + forward);


                        //                        ComponentExtensions.ApplyLinearImpulse
                        //                     (ref velocity,
                        //                      mass,
                        //                   gunScript.AmmoStartLocation.forward * (gun.Strength + math.abs(playerVelocity.Linear.x)));

                        //ComponentExtensions.ApplyLinearImpulse
                        //(ref velocity,
                        //    mass,
                        //    translate * (gun.Strength + math.abs(playerVelocity.Linear.x)));

                        // Direction Functions
                        //Matrix4x4 version of Transform.InverseTransformDirection

                        //Matrix4x4 matrix4x4 = Matrix4x4.identity;
                        //matrix4x4.SetTRS(translate, rotation, Vector3.one);

                        //Vector3 localDirection = Matrix4x4.Inverse(MultiplyVector(worldDirection);

                        velocity.Linear = forward * (gun.Strength + math.abs(playerVelocity.Linear.x));
                        //velocity.Linear = forward * gun.Strength;



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
