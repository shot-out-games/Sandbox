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




//[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

//[UpdateAfter(typeof(BuildPhysicsWorld))]
//[UpdateBefore(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

public class RaycastSystem : SystemBase
{



















    protected override void OnUpdate()
    {



        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity, ref ApplyImpulseComponent applyImpulse,
            ref Translation translation, ref PhysicsVelocity pv, ref PhysicsCollider collider, ref Rotation rotation, in PlayerComponent playerComponent) =>
        {

            var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
            var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

            applyImpulse.Grounded = true;
            applyImpulse.BumpLeft = false;
            applyImpulse.BumpRight = false;

            float3 start = translation.Value + new float3(0f,  .38f, 0);
            float3 direction = new float3(0, 0, 0);
            float distance = .38f;
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

            bool hasPointHit = collisionWorld.CalculateDistance(pointDistanceInput, out DistanceHit pointHit);

            hasPointHit = false;


            if (hasPointHit && applyImpulse.InJump == true)
            {
                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[pointHit.RigidBodyIndex].Entity;
                //Debug.Log("ve " + applyImpulse.Velocity.x);
                if (applyImpulse.Velocity.x < 0)
                {
                    applyImpulse.Grounded = false;
                    applyImpulse.BumpLeft = true;
                    Debug.Log("bump left ");
                }
                else if (applyImpulse.Velocity.x > 0)
                {
                    applyImpulse.Grounded = false;
                    applyImpulse.BumpRight = true;
                    Debug.Log("bump right ");
                }

                //Debug.Log("pt pos " + pointHit.Position + " pt fraction " + pointHit.Fraction + " pt e " + e);
            }

            else
            {

                start = translation.Value + new float3(0, 0, 0);
                direction = new float3(0, -1, 0);
                distance = .61f;
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
                    //Debug.Log("start " + start +  "end " + end + " fraction " + hitDown.Fraction);



                    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitDown.RigidBodyIndex].Entity; //grounded
                    Debug.Log("hd " + hitDown.Entity);

                    //Filter replaces tag
                    //if (EntityManager.HasComponent(e, typeof(TriggerComponent)))
                    //{
                    //Debug.Log("pt " + e);
                    //    if (EntityManager.GetComponentData<TriggerComponent>(e).Type == (int)TriggerType.Ground)
                    //    {
                    //        applyImpulse.Grounded = true;
                    //    }
                    //}
                    if (applyImpulse.InJump == true)
                    {
                        applyImpulse.InJump = false;
                        applyImpulse.Grounded = true;
                    }

                    applyImpulse.Falling = false;

                }
                else
                {
                    if (applyImpulse.InJump == false)
                    {
                        applyImpulse.Falling = true;
                    }

                    applyImpulse.Grounded = false;

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
                        CollidesWith = 2,
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
                    //Debug.Log("hu " + hitUp.Entity);
                    //applyImpulse.Ceiling = true;

                }



                //start = translation.Value + new float3(.19f, .5f, 0);
                //direction = new float3(.19f, 0, 0);
                //distance = 1.0f;
                //end = start + direction * distance;
                //RaycastInput inputRight = new RaycastInput()
                //{
                //    Start = start,
                //    End = end,
                //    Filter = new CollisionFilter()
                //    {
                //        BelongsTo = 1,
                //        CollidesWith = 2,
                //        GroupIndex = 0
                //    }
                //};
                //Debug.DrawRay(inputRight.Start, direction, Color.red, distance);

                //Unity.Physics.RaycastHit hitRight = new Unity.Physics.RaycastHit();

                //bool hasPointHitRight = collisionWorld.CastRay(inputRight, out hitRight);



                //if (hasPointHitRight)
                //{

                //    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitUp.RigidBodyIndex].Entity;//grounded
                //    //Debug.Log("hr " + hitRight.Entity);
                //    //applyImpulse.BumpRight = true;

                //}






                //start = translation.Value + new float3(-.19f, .5f, 0);
                //direction = new float3(-.19f, 0f, 0);
                //distance = 1.0f;
                //end = start + direction * distance;
                ////end = start + pv.Linear * Time.DeltaTime;

                //RaycastInput inputLeft = new RaycastInput()
                //{
                //    Start = start,
                //    End = end,
                //    //Filter = CollisionFilter.Default
                //    Filter = new CollisionFilter()
                //    {
                //        BelongsTo = 1,
                //        CollidesWith = 2,
                //        GroupIndex = 0
                //    }
                //};
                //Debug.DrawRay(inputLeft.Start, direction, Color.blue, distance);
                //Unity.Physics.RaycastHit hitLeft = new Unity.Physics.RaycastHit();

                //bool hasPointHitLeft = collisionWorld.CastRay(inputLeft, out hitLeft);



                //if (hasPointHitLeft)
                //{

                //    Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hitLeft.RigidBodyIndex].Entity;//grounded
                //    //Debug.Log("hl " + hitLeft.Entity);
                //    //applyImpulse.BumpLeft = true;

                //}





                ////start = translation.Value + new float3(-.19f, .5f, 0);
                ////direction = new float3(-1.0f, 0f, 0);
                ////distance = 1.0f;
                ////end = start + direction * distance;
                ////SphereCast(ref collider, ref applyImpulse, start, end, direction, distance);




            }









        }).Run();




        bool key = false;


        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity,
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

            bool haveHit = collisionWorld.CalculateDistance(input, out DistanceHit hit);



            //RaycastInput input = new RaycastInput
            //{
            //    Start = translation.Value + new float3(-1, 5, 0),
            //    End = translation.Value + new float3(1, 5, 0),
            //    Filter = CollisionFilter.Default
            //};


            //Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();
            //bool haveHit = collisionWorld.CalculateDistance(input, out hit);
            if (haveHit)
            {


                Entity e = physicsWorldSystem.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                //Debug.Log("e " + e);
                if (EntityManager.HasComponent(e, typeof(TriggerComponent)))
                {
                    if (EntityManager.GetComponentData<TriggerComponent>(e).Type == (int)TriggerType.Trigger
                        && EntityManager.GetComponentData<TriggerComponent>(e).index == LevelManager.instance.currentLevel + 1

                    )
                    {
                        levelComplete.targetReached = true;
                    }

                    if (EntityManager.GetComponentData<TriggerComponent>(e).Type == (int)TriggerType.Key
                    )
                    {
                        winnerComponent.keys += 1;
                        key = true;
                        EntityManager.DestroyEntity(e);
                    }



                }
            }








        }).Run();











        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity entity,

                 HudGroup hudGroup

            ) =>
        {

            if (key == true)
            {
                key = false;
                hudGroup.cubes -= 1;
                hudGroup.ShowLabelLevelTargets();
            }







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


