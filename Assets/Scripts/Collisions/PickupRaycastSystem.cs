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



        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, 
            ref Translation translation, ref PhysicsVelocity pv, ref PhysicsCollider collider, ref Rotation rotation, in PlayerComponent playerComponent) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


            float3 start = translation.Value + new float3(0f, .38f, 0);
            float3 direction = new float3(0, 0, 0);
            float distance = .38f;
            float3 end = start + direction * distance;


            PointDistanceInput pointDistanceInput = new PointDistanceInput
            {
                Position = start,
                MaxDistance = distance,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
//                    BelongsTo = (uint)CollisionLayer.Player,
   //                 CollidesWith = (uint)CollisionLayer.Item,
                    BelongsTo = 1,
                    CollidesWith = 4,
                    GroupIndex = 0
                }
            };

            

            bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);



            if (hasPointHit)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
                //Debug.Log("ve " + applyImpulse.Velocity.x);
                //Debug.Log("left / right");
                Debug.Log(" pt e " + e);
            }


            start = translation.Value + new float3(0, .03f, 0);
            direction = new float3(0, -1, 0);
            distance = .35f;
            end = start + direction * distance;



            RaycastInput inputDown = new RaycastInput()
            {
                Start = start,
                End = end,
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1,
                    CollidesWith = 4,
                    GroupIndex = 0
                }
            };
            Unity.Physics.RaycastHit hitDown = new Unity.Physics.RaycastHit();
            Debug.DrawRay(inputDown.Start, direction, Color.yellow, distance);

            bool hasPointHitDown = collisionWorld.CastRay(inputDown, out hitDown);


            if (hasPointHitDown)
            {
                Debug.Log("down");
                //Debug.Log("start " + start +  "end " + end + " fraction " + hitDown.Fraction);
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitDown.RigidBodyIndex].Entity; //grounded
            }

            start = translation.Value + new float3(0, 1f, 0);
            direction = new float3(0, 1f, 0);
            distance = .19f;
            end = start + direction * distance;
            //end = start + pv.Linear * Time.DeltaTime;

            RaycastInput inputUp = new RaycastInput()
            {
                Start = start,
                End = end,
                //Filter = CollisionFilter.Default
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1,
                    CollidesWith = 4,
                    GroupIndex = 0
                }
            };
            Debug.DrawRay(inputUp.Start, direction, Color.green, distance);
            //Debug.Log("st " + inputUp.Start);
            //Debug.Log("en " + inputUp.End);

            Unity.Physics.RaycastHit hitUp = new Unity.Physics.RaycastHit();

            bool hasPointHitUp = collisionWorld.CastRay(inputUp, out hitUp);



            if (hasPointHitUp)
            {

                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitUp.RigidBodyIndex].Entity; //grounded
                Debug.Log("hu " + hitUp.Entity);
                //applyImpulse.Ceiling = true;

            }








        }).Run();









    }




}


