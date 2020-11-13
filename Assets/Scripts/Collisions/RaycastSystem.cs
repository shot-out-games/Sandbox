﻿using SandBox.Player;
using TMPro;
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



public class RaycastSystem : SystemBase
{





    protected override void OnUpdate()
    {



        Entities.WithoutBurst().ForEach((Entity entity, ref ApplyImpulseComponent applyImpulse,
            ref Translation translation, ref PhysicsVelocity pv, ref PhysicsCollider collider, ref Rotation rotation, in PlayerComponent playerComponent) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            applyImpulse.Grounded = true;
            applyImpulse.BumpLeft = false;
            applyImpulse.BumpRight = false;

            float3 start = translation.Value + new float3(0f, .4f, 0);
            float3 direction = new float3(0, 0, 0);
            float distance = .4f;
            float3 end = start + direction * distance;



            PointDistanceInput pointDistanceInput = new PointDistanceInput
            {
                Position = start,
                MaxDistance = distance,
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1,
                    CollidesWith = 2,
                    GroupIndex = 0
                }
            };

            bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);//bump left / right n/a

            hasPointHit = false;


            if (hasPointHit && applyImpulse.InJump == true)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
                if (applyImpulse.Velocity.x < 0)
                {
                    applyImpulse.Grounded = false;
                    applyImpulse.BumpLeft = true;
                }
                else if (applyImpulse.Velocity.x > 0)
                {
                    applyImpulse.Grounded = false;
                    applyImpulse.BumpRight = true;
                }

            }
            else
            {

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
                        CollidesWith = 2,
                        GroupIndex = 0
                    }
                };
                Unity.Physics.RaycastHit hitDown = new Unity.Physics.RaycastHit();
                Debug.DrawRay(inputDown.Start, direction, Color.yellow, distance);

                bool hasPointHitDown = collisionWorld.CastRay(inputDown, out hitDown);


                if (hasPointHitDown)
                {
                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitDown.RigidBodyIndex].Entity; //grounded
                    if (applyImpulse.InJump == true)
                    {
                        applyImpulse.InJump = false;
                        applyImpulse.Grounded = true;
                    }

                    applyImpulse.fallingFramesCounter = 0;
                    applyImpulse.Falling = false;

                }
                else
                {
                    if (applyImpulse.InJump == false)
                    {
                        if (applyImpulse.fallingFramesCounter > applyImpulse.fallingFramesMaximuim)
                        {
                            applyImpulse.Falling = true;
                        }
                        else
                        {
                            applyImpulse.Falling = false;
                            applyImpulse.fallingFramesCounter++;
                        }
                    }

                    applyImpulse.Grounded = false;

                }

                start = translation.Value + new float3(0, 1f, 0);
                direction = new float3(0, 1f, 0);
                distance = .20f;
                end = start + direction * distance;

                RaycastInput inputUp = new RaycastInput()
                {
                    Start = start,
                    End = end,
                    Filter = new CollisionFilter()
                    {
                        BelongsTo = 1,
                        CollidesWith = 2,
                        GroupIndex = 0
                    }
                };

                Unity.Physics.RaycastHit hitUp = new Unity.Physics.RaycastHit();

                bool hasPointHitUp = collisionWorld.CastRay(inputUp, out hitUp);



                if (hasPointHitUp)
                {
                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitUp.RigidBodyIndex].Entity; //ceiling n/a
                }



            }


        }).Run();


        bool key = false;

        Entities.WithoutBurst().ForEach((Entity entity,
            ref LevelCompleteComponent levelComplete, ref WinnerComponent winnerComponent,
            ref Translation translation, ref PhysicsVelocity pv, ref Rotation rotation, in PlayerComponent playerComponent) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


            PointDistanceInput input = new PointDistanceInput
            {
                Position = translation.Value,
                MaxDistance = 2f,
                Filter = new CollisionFilter()
                {
                    BelongsTo = 1,
                    CollidesWith = 2,
                    GroupIndex = 0
                }
            };

            bool haveHit = collisionWorld.CalculateDistance(input, out DistanceHit hit);//trigger hit

            if (haveHit)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                if (HasComponent<TriggerComponent>(e))
                {
                    if (GetComponent<TriggerComponent>(e).Type == (int)TriggerType.Trigger
                        && GetComponent<TriggerComponent>(e).index == LevelManager.instance.currentLevelCompleted + 1

                    )
                    {
                        levelComplete.targetReached = true;
                    }

                    if (GetComponent<TriggerComponent>(e).Type == (int)TriggerType.Key &&
                    GetComponent<TriggerComponent>(e).Active == true
                    )
                    {
                        winnerComponent.keys += 1;
                        key = true;
                        TriggerComponent trigger = GetComponent<TriggerComponent>(e);
                        trigger.Hit = true;
                        SetComponent<TriggerComponent>(e, trigger);
                        LevelManager.instance.audioSourceGame.Stop();

                    }

                }



            }


        }).Run();





        Entities.WithoutBurst().ForEach((
            ref TriggerComponent triggerComponent,
            in Trigger triggerMB

        ) =>
        {

            if (triggerComponent.Hit && triggerComponent.Active == true)
            {
                triggerComponent.Active = false;
                Debug.Log("active ");

                if (triggerMB.triggerParticleSystem != null)
                {
                    triggerMB.triggerParticleSystem.Play(true);
                }
                if (triggerMB.triggerAudioSource != null)
                {

                    triggerMB.triggerAudioSource.Play();
                    Debug.Log("active audio source play ");
                }


            }
            else if (key == true && triggerMB.triggerAudioSource != null)
            {
                triggerMB.triggerAudioSource.Stop();
                Debug.Log("active audio source play  stop ");
            }



            triggerComponent.Hit = false;

        }).Run();


    }


    public unsafe Entity SphereCast(ref PhysicsCollider physicsCollider, ref ApplyImpulseComponent applyImpulse, float3 start, float3 end, float3 direction, float distance)
    {

        var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;


        //end = start + pv.Linear * Time.DeltaTime;

        var filter = new CollisionFilter()
        {
            BelongsTo = 1,
            CollidesWith = 2,
            GroupIndex = 0
        };




        SphereCollider* sphere = (SphereCollider*)physicsCollider.ColliderPtr;
        sphere->Filter = filter;

        // update the collider geometry
        var sphereGeometry = sphere->Geometry;
        //sphereGeometry.Radius = 5.0f;
        sphere->Geometry = sphereGeometry;

        ColliderCastInput inputLeft = new ColliderCastInput()
        {
            Start = start,
            End = end,
            Orientation = quaternion.identity,
            Collider = (Unity.Physics.Collider*)sphere
        };
        Debug.DrawRay(inputLeft.Start, direction, Color.black, distance);

        Unity.Physics.RaycastHit hitLeft = new Unity.Physics.RaycastHit();

        bool hasPointHitLeft = collisionWorld.CastCollider(inputLeft, out ColliderCastHit colliderHit);


        if (hasPointHitLeft)
        {

            Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitLeft.RigidBodyIndex].Entity;//grounded
            Debug.Log("sphere l " + hitLeft.Entity);
            applyImpulse.BumpLeft = true;

        }


        return Entity.Null;


    }






}


