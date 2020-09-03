using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.Transforms;


//[UpdateAfter(typeof(StepPhysicsWorld))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


//[UpdateAfter(typeof(BuildPhysicsWorld))]
//[UpdateBefore(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]



public class MoveCollisionSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }





    struct MoveCollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentDataFromEntity<TriggerComponent> triggerGroup;
        [ReadOnly] public ComponentDataFromEntity<ApplyImpulseComponent> characterGroup;
        [ReadOnly] public ComponentDataFromEntity<Translation> translationGroup;
        public EntityCommandBuffer CommandBuffer;
        public void Execute(CollisionEvent ev) // this is never called
        {

            //Entity a = physicsWorld.Bodies[ev.BodyIndexA].Entity;
            //Entity b = physicsWorld.Bodies[ev.BodyIndexB].Entity;


            //if (triggerGroup.HasComponent(a) == false || triggerGroup.HasComponent(b) == false) return;
            //var triggerComponent_a = triggerGroup[a];
            //var triggerComponent_b = triggerGroup[b];

            ////Debug.Log("a " + triggerComponent_a.Type + " b " + triggerComponent_b.Type);

            //Entity ch_a = triggerComponent_a.ParentEntity;
            //Entity ch_b = triggerComponent_b.ParentEntity;


            //if (ch_a == ch_b) return;


            //if (triggerComponent_a.Type == triggerComponent_b.Type) return;



            //if (characterGroup.HasComponent(a) == true)
            //{
            //    var impulseComponent_a = characterGroup[a];

            //    if (triggerComponent_b.Type == (int)TriggerType.Ground)
            //    {
            //        //impulseComponent_a.Grounded = true;
            //        impulseComponent_a.LastPositionLand = translationGroup[a].Value;
            //        //Debug.Log(impulseComponent_a.LastPositionLand);
            //        CommandBuffer.SetComponent(ch_a, impulseComponent_a);

            //    }
            //    else
            //    {
            //        //Debug.Log("air a");
            //        return;
            //    }
            //}

            //if (characterGroup.HasComponent(b) == true)
            //{
            //    var impulseComponent_b = characterGroup[b];

            //    if (triggerComponent_a.Type == (int)TriggerType.Ground)
            //    {
            //        //impulseComponent_b.Grounded = true;
            //        impulseComponent_b.LastPositionLand = translationGroup[b].Value;
            //        //Debug.Log(impulseComponent_b.LastPositionLand);
            //        CommandBuffer.SetComponent(ch_b, impulseComponent_b);

            //    }
            //    else
            //    {
            //        //Debug.Log("air a");
            //        return;
            //    }
            //}




        }
    }


    BuildPhysicsWorld buildPhysicsWorldSystem;
    StepPhysicsWorld stepPhysicsWorld;
    EndFramePhysicsSystem endFramePhysicsSystem;


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        //Entities.ForEach(
        //    (ref ApplyImpulseComponent applyImpulse) =>
        //    {
        //        applyImpulse.Grounded = false;
        //    }).Schedule(inputDeps);


        inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorldSystem.GetOutputDependency());
        inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.GetOutputDependency());
        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;
        var collisionJob = new MoveCollisionJob
        {
            physicsWorld = physicsWorld,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            triggerGroup = GetComponentDataFromEntity<TriggerComponent>(true),
            characterGroup = GetComponentDataFromEntity<ApplyImpulseComponent>(true),
            translationGroup = GetComponentDataFromEntity<Translation>(true)
        };
        JobHandle collisionHandle = collisionJob.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);
        collisionHandle.Complete();
        return collisionHandle;
    } // OnUpdate




} // System


