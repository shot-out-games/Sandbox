using RootMotion.FinalIK;
using SandBox.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using SphereCollider = Unity.Physics.SphereCollider;




[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]



public class PickupWeaponRaycastSystem : SystemBase
{


    public enum CollisionLayer
    {
        Player = 1 << 0,
        Ground = 1 << 1,
        Enemy = 1 << 2,
        Item = 1 << 3
    }


    protected override void OnUpdate()
    {
        bool pickedUp = false;
        Entity pickedUpEntity = Entity.Null;
        InteractionObject interactionObject = null;
        Entity pickerUpper = Entity.Null;


        var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();



        //        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, WeaponItem WeaponItem, ref WeaponItemComponent weaponItemComponent,
        //         ref Translation translation, ref PhysicsCollider collider, ref Rotation rotation) =>
        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, WeaponItem WeaponItem,
                ref Translation translation, ref PhysicsCollider collider, ref Rotation rotation) =>
        {

            if (bufferFromEntity.HasComponent(entity))
            {
                //var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();
                var weaponItemComponent = bufferFromEntity[entity];


                var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
                var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


                float3 start = translation.Value + new float3(0f, .38f, 0);
                float3 direction = new float3(0, 0, 0);
                float distance = 2f;
                float3 end = start + direction * distance;


                PointDistanceInput pointDistanceInput = new PointDistanceInput
                {
                    Position = start,
                    MaxDistance = distance,
                    //Filter = CollisionFilter.Default
                    Filter = new CollisionFilter()
                    {
                        //BelongsTo = (uint)CollisionLayer.Player,
                        //CollidesWith = (uint)CollisionLayer.Item,
                        BelongsTo = 4u,
                        CollidesWith = 1u,
                        GroupIndex = 0
                    }
                };

                Debug.DrawRay(start, Vector3.right, Color.green, distance * 10f);





                bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);
                if (HasComponent<TriggerComponent>(pointHit.Entity))
                {
                    var parent = EntityManager.GetComponentData<TriggerComponent>(pointHit.Entity).ParentEntity;
                    pickerUpper = parent;
                    Debug.Log(" pt e " + pickerUpper);

                }


                if (hasPointHit && weaponItemComponent[0].pickedUp == false)
                {


                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
                    //Debug.Log("ve " + applyImpulse.Velocity.x);
                    //Debug.Log("left / right");
                    if (WeaponItem.e == entity)
                    {
                        //weaponItemComponent.pickedUp = true;

                        var intBufferElement = weaponItemComponent[0];
                        intBufferElement.pickedUp = true;
                        weaponItemComponent[0] = intBufferElement;

                        pickedUp = true;
                        pickedUpEntity = entity;
                        interactionObject = WeaponItem.interactionObject;
                        //Debug.Log(" pt e " + e);
                    }
                }




            }




        }).Run();


        if (pickedUp)
        {



            Entities.WithoutBurst().ForEach((WeaponInteraction weaponInteraction, WeaponManager weaponManager, Entity e) =>
            {
                if (pickerUpper == e)
                {
                    weaponManager.DetachPrimaryWeapon(); //need to add way to set to not picked up  afterwards
                    weaponInteraction.interactionObject = interactionObject;
                    //weaponManager.primaryWeapon.weaponGameObject = interactionObject.gameObject;
                    weaponManager.primaryWeapon.weaponGameObject =
                        interactionObject.GetComponent<WeaponItem>().gameObject; //always the same as go for now
                    weaponManager.AttachPrimaryWeapon();
                    weaponInteraction.UpdateSystem();
                    Debug.Log("MATCH FOUND");
                }
                else
                {
                    pickedUp = false;
                    var weaponItemBufferList = GetBufferFromEntity<WeaponItemComponent>();
                    var weaponItemComponent = bufferFromEntity[pickedUpEntity];
                    var intBufferElement = weaponItemComponent[0];
                    intBufferElement.pickedUp = false;
                    weaponItemComponent[0] = intBufferElement;

                }

            }).Run();

            //EntityManager.DestroyEntity(pickedUpEntity);

        }

        if (pickedUp == true)
        {
            EntityManager.SetComponentData(pickedUpEntity, new Translation { Value = new float3(0, -2500, 0) });
        }


    }




}


public class PickupPowerUpRaycastSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        bool pickedUp = false;
        Entity pickedUpActor = Entity.Null;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Entities.WithoutBurst().ForEach((
            ref PowerItemComponent powerItemComponent,
            in Translation translation,
            in Entity entity
        ) =>
        {


            var physicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            float3 start = translation.Value + new float3(0f, .38f, 0);
            float3 direction = new float3(0, 0, 0);
            float distance = 2f;
            float3 end = start + direction * distance;


            PointDistanceInput pointDistanceInput = new PointDistanceInput
            {
                Position = start,
                MaxDistance = distance,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = 7u,
                    CollidesWith = 1u,
                    GroupIndex = 0
                }
            };



            bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);


            if (hasPointHit)
            {
                if (HasComponent<TriggerComponent>(pointHit.Entity))
                {
                    var parent = GetComponent<TriggerComponent>(pointHit.Entity).ParentEntity;
                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
                    pickedUpActor = parent;
                    Debug.Log(" pt e " + pickedUpActor);
                    //if (HasComponent<PowerItemComponent>(parent) == false)
                    //{
                    //var powerItem = GetComponent<PowerItemComponent>(entity);
                    if (powerItemComponent.powerType == (int)PowerType.Health &&
                        HasComponent<HealthPower>(pickedUpActor) == false && HasComponent<DestroyComponent>(entity) == false)
                    {
                        powerItemComponent.enabled = true;
                        Entity instanceEntity = ecb.Instantiate(powerItemComponent.particleSystemEntity);
                        var ps = new ParticleSystemComponent
                        {
                            followActor = true,
                            pickedUpActor = pickedUpActor
                        };

                        ecb.AddComponent(instanceEntity, ps);

                        Debug.Log(" health " + pickedUpActor);

                        HealthPower healthPower = new HealthPower
                        {
                            psAttached = instanceEntity,//attached to player picking up
                            pickedUpActor = pickedUpActor,
                            itemEntity = entity,
                            enabled = true,
                            healthMultiplier = powerItemComponent.healthMultiplier
                        };
                        ecb.AddComponent(pickedUpActor, healthPower);

                    }



                    if (powerItemComponent.powerType == (int)PowerType.Speed &&
                        HasComponent<Speed>(pickedUpActor) == false && HasComponent<DestroyComponent>(entity) == false)
                    {



                        powerItemComponent.enabled = true;
                        Entity instanceEntity = ecb.Instantiate(powerItemComponent.particleSystemEntity);

                        var ps = new ParticleSystemComponent
                        {
                            followActor = true,
                            pickedUpActor = pickedUpActor
                        };

                        ecb.AddComponent(instanceEntity, ps);

                        Debug.Log(" speed " + pickedUpActor);


                        Speed speedPower = new Speed
                        {
                            psAttached = instanceEntity,//attached to player on health pick up
                            pickedUpActor = pickedUpActor,
                            itemEntity = entity,
                            enabled = true,
                            timeOn = powerItemComponent.speedTimeOn,
                            multiplier = powerItemComponent.speedTimeMultiplier
                        };

                        ecb.AddComponent(pickedUpActor, speedPower);

                    }
                    // }
                }
            }



        }).Run();

        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);


    }





}


