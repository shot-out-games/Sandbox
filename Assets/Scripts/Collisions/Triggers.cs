using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections;

public struct TriggerComponent : IComponentData
{
    public int key;
    public int Type;
    public int index;
    public int CurrentFrame;
    public bool triggerChecked;
    //parent of trigger ie bone 
    //if trigger is bullet then just returns bullet not shooter
    //use ammo component for shooter (owner)
    public Entity ParentEntity;
}

//[InternalBufferCapacity(8)]
public struct CollisionComponent : IComponentData
{
    public int Part_entity;
    public int Part_other_entity;
    public Entity Character_entity;
    public Entity Character_other_entity;
    //public float currentFrame;
}

public struct PowerTriggerComponent : IComponentData
{
    public int TriggerType;
}


//[UpdateBefore(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]

//[UpdateAfter(typeof(BuildPhysicsWorld))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]


public class CollisionSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

    }


    struct CollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentDataFromEntity<TriggerComponent> triggerGroup;
        public EntityCommandBuffer CommandBuffer;
        public void Execute(CollisionEvent ev) // this is never called
        {

            Entity a = physicsWorld.Bodies[ev.BodyIndexA].Entity;
            Entity b = physicsWorld.Bodies[ev.BodyIndexB].Entity;
            //Debug.Log("ena " + a + " enb " + b);

            if (triggerGroup.HasComponent(a) == false || triggerGroup.HasComponent(b) == false) return;
            var triggerComponent_a = triggerGroup[a];
            var triggerComponent_b = triggerGroup[b];

            Entity ch_a = triggerComponent_a.ParentEntity;
            Entity ch_b = triggerComponent_b.ParentEntity;

            //if (ch_a == ch_b) return;

            //Debug.Log("cha " + triggerComponent_a.Type + " chb " + triggerComponent_b.Type);

            if (triggerComponent_a.Type == triggerComponent_b.Type) return;

            CollisionComponent collisionComponent =
                 new CollisionComponent()
                 {
                     Part_entity = triggerComponent_a.Type,
                     Part_other_entity = triggerComponent_b.Type,
                     Character_entity = ch_a,
                     Character_other_entity = ch_b
                 };

            CommandBuffer.AddComponent(ch_a, collisionComponent);
            CommandBuffer.AddComponent(ch_b, collisionComponent);

            

                if (triggerComponent_a.Type == (int)TriggerType.Lever || triggerComponent_b.Type == (int)TriggerType.Lever)
                {
                    //Debug.Log("type a " + collisionComponent.Part_entity + " type b " + collisionComponent.Part_other_entity + " cha " + ch_a + " chb " + ch_b);
                }


            if (collisionComponent.Part_entity == (int)TriggerType.Ammo && collisionComponent.Part_other_entity == (int)TriggerType.Blocks)
            {
                CommandBuffer.AddComponent(ch_a, new BlockComponent() { blocked = true });
            }

            if (collisionComponent.Part_entity == (int)TriggerType.Blocks && collisionComponent.Part_other_entity == (int)TriggerType.Ammo)
            {
                CommandBuffer.AddComponent(ch_b, new BlockComponent() { blocked = true });
            }

            if (collisionComponent.Part_entity == (int)TriggerType.Chest && collisionComponent.Part_other_entity == (int)TriggerType.PowerupControl)
            {
                CommandBuffer.AddComponent(ch_a,
                    new PowerTriggerComponent { TriggerType = collisionComponent.Part_entity });
            }

            if (collisionComponent.Part_entity == (int)TriggerType.PowerupControl && collisionComponent.Part_other_entity == (int)TriggerType.Chest)
            {
                CommandBuffer.AddComponent(ch_b, new PowerTriggerComponent { TriggerType = collisionComponent.Part_other_entity });
            }

            
        }
    }


    BuildPhysicsWorld buildPhysicsWorldSystem;
    StepPhysicsWorld stepPhysicsWorld;
    EndFramePhysicsSystem endFramePhysicsSystem;


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Entities.WithoutBurst().ForEach(
            (ref TriggerComponent triggerComponent) =>
            {
                triggerComponent.CurrentFrame++;
            }).Run();


        inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorldSystem.GetOutputDependency());
        inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.GetOutputDependency());
        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;
        var collisionJob = new CollisionJob
        {
            physicsWorld = physicsWorld,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            triggerGroup = GetComponentDataFromEntity<TriggerComponent>(true),
        };
        JobHandle collisionHandle = collisionJob.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);
        collisionHandle.Complete();
        return collisionHandle;
    } // OnUpdate




} // System


