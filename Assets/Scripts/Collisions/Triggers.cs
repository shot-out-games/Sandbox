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


public struct CheckedComponent : IComponentData
{
    public bool collisionChecked;

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
            int type_a = triggerComponent_a.Type;
            int type_b = triggerComponent_b.Type;


            if (ch_a == ch_b) return;////?????


            if (type_a == type_b) return;


            if (triggerComponent_a.Type == (int)TriggerType.Ground || triggerComponent_b.Type == (int)TriggerType.Ground) return;



            bool punchingA = (type_b == (int)TriggerType.Chest || type_b == (int)TriggerType.Head) &&
                             (type_a == (int)TriggerType.LeftHand || type_a == (int)TriggerType.RightHand
                                                                  || type_a == (int)TriggerType.LeftFoot ||
                                                                  type_a == (int)TriggerType.RightFoot);

            bool punchingB = (type_a == (int)TriggerType.Chest || type_a== (int)TriggerType.Head) &&
                            (type_b == (int)TriggerType.LeftHand || type_b == (int)TriggerType.RightHand
                                                                 || type_b == (int)TriggerType.LeftFoot ||
                                                                 type_b == (int)TriggerType.RightFoot);

            bool ammoA = (type_b == (int)TriggerType.Chest || type_b == (int)TriggerType.Head) &&
                         (type_a == (int)TriggerType.Ammo);

            bool ammoB = (type_a == (int)TriggerType.Chest || type_a == (int)TriggerType.Head) &&
                         (type_b == (int)TriggerType.Ammo);



            if (punchingA || ammoA)
            {


                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_a.Type,
                        Part_other_entity = triggerComponent_b.Type,
                        Character_entity = ch_a,
                        Character_other_entity = ch_b
                    };
                CommandBuffer.AddComponent(ch_a, collisionComponent);
            }
            else if (punchingB || ammoB)
            {

                //Debug.Log("t b " + triggerComponent_b.Type + " t a " + triggerComponent_a.Type);
                //Debug.Log("c b " + ch_b + " c a " + ch_a);


                Debug.Log("a " + ammoA + " b " + ammoB);


                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_b.Type,
                        Part_other_entity = triggerComponent_a.Type,
                        Character_entity = ch_b,
                        Character_other_entity = ch_a
                    };
                CommandBuffer.AddComponent(ch_b, collisionComponent);
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


