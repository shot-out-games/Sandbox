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



public class PickupRaycastSystem : SystemBase
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

        var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();

        //        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, WeaponItem WeaponItem, ref WeaponItemComponent weaponItemComponent,
        //         ref Translation translation, ref PhysicsCollider collider, ref Rotation rotation) =>
        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, WeaponItem WeaponItem, 
                ref Translation translation, ref PhysicsCollider collider, ref Rotation rotation) =>
        {

            if (bufferFromEntity.HasComponent(entity))
            {
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
                Debug.Log(" pt e " + pointHit.ColliderKey);



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
                        Debug.Log(" pt e " + e);
                    }
                }


                start = translation.Value + new float3(0, .03f, 0);
                direction = new float3(0, -1, 0);
                distance = .35f;
                end = start + direction * distance;



            }




        }).Run();


        if (pickedUp)
        {
            Entities.WithoutBurst().ForEach((WeaponInteraction weaponInteraction) =>
            {
                weaponInteraction.interactionObject = interactionObject;
                weaponInteraction.UpdateSystem();
                Debug.Log("MATCH FOUND");

            }).Run();

            //EntityManager.DestroyEntity(pickedUpEntity);
            
        }




    }




}


