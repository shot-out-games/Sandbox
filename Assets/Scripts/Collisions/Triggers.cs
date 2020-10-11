using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System;
using SandBox.Player;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

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
    public Entity Entity;
    public Entity ParentEntity;
    public bool Hit;
    public bool Active;
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
    public bool isMelee;
    //public float currentFrame;
}

public struct PowerTriggerComponent : IComponentData
{
    public int TriggerType;
}


//[UpdateBefore(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]

//[UpdateAfter(typeof(BuildPhysicsWorld))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PlayerMoveSystem))]
//[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]

//[UpdateInGroup(typeof(SimulationSystemGroup))]


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
        [ReadOnly] public ComponentDataFromEntity<MeleeComponent> meleeGroup;
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



            bool anyPVEtouchA = false;
            bool anyPVEtouchB = false;
            if (meleeGroup.HasComponent(ch_a) == true)
            {
                var meleeComponent_a = meleeGroup[ch_a];
                if (meleeComponent_a.anyTouchDamage == true)
                {
                    anyPVEtouchA = true;
                }
            }
            if (meleeGroup.HasComponent(ch_b) == true)
            {
                var meleeComponent_b = meleeGroup[ch_b];
                if (meleeComponent_b.anyTouchDamage == true)
                {
                    anyPVEtouchB = true;
                }

            }





            if (triggerComponent_a.Type == (int)TriggerType.Ground ||
                triggerComponent_b.Type == (int)TriggerType.Ground)
            {
                return;
            }


            bool punchingA = false;
            bool punchingB = false;
            if (anyPVEtouchA == true &&
                (type_a == (int)TriggerType.Body || type_a == (int)TriggerType.Base) || type_a == (int)TriggerType.Head)
            {
                punchingA = true;
                Debug.Log("cha " + ch_a);
            }
            else if (anyPVEtouchB == true && 
                (type_b == (int)TriggerType.Body || type_b == (int)TriggerType.Base) || type_b == (int)TriggerType.Head)

            {
                punchingB = true;
                Debug.Log("chb" + ch_b);
            }



            if (type_a == type_b && punchingA == false && punchingB == false) return;

            bool meleeA = false;
            bool meleeB = false;


            meleeA = (type_b == (int)TriggerType.Base || type_b == (int)TriggerType.Head) &&
    (type_a == (int)TriggerType.Melee);

            meleeB = (type_a == (int)TriggerType.Base || type_a == (int)TriggerType.Head) &&
    (type_b == (int)TriggerType.Melee);



            punchingA = (type_b == (int)TriggerType.Base || type_b == (int)TriggerType.Head) &&
                (type_a == (int)TriggerType.LeftHand || type_a == (int)TriggerType.RightHand
                                                     || type_a == (int)TriggerType.LeftFoot ||
                                                     type_a == (int)TriggerType.RightFoot) || punchingA;


            punchingB = (type_a == (int)TriggerType.Base || type_a == (int)TriggerType.Head) &&
                            (type_b == (int)TriggerType.LeftHand || type_b == (int)TriggerType.RightHand
                                                                 || type_b == (int)TriggerType.LeftFoot ||
                                                                 type_b == (int)TriggerType.RightFoot) || punchingB;








            bool ammoA = (type_b == (int)TriggerType.Base || type_b == (int)TriggerType.Head || type_b == (int)TriggerType.Body) &&
                         (type_a == (int)TriggerType.Ammo);

            bool ammoB = (type_a == (int)TriggerType.Base || type_a == (int)TriggerType.Head || type_a == (int)TriggerType.Body) &&
                         (type_b == (int)TriggerType.Ammo);

            //Debug.Log("aa " + ammoA + " ab " + ammoB);

            if (punchingA || ammoB || meleeA)
            {

                Debug.Log("t b " + triggerComponent_b.Type + " t a " + triggerComponent_a.Type);
                Debug.Log("c b " + ch_b + " c a " + ch_a);

                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_a.Type,
                        Part_other_entity = triggerComponent_b.Type,
                        Character_entity = ch_a,
                        Character_other_entity = ch_b,
                        isMelee = meleeA
                    };
                CommandBuffer.AddComponent(ch_a, collisionComponent);
            }
            else if (punchingB || ammoA || meleeB)
            {

                //Debug.Log("t b " + triggerComponent_b.Type + " t a " + triggerComponent_a.Type);
                //Debug.Log("c b " + ch_b + " c a " + ch_a);




                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_b.Type,
                        Part_other_entity = triggerComponent_a.Type,
                        Character_entity = ch_b,
                        Character_other_entity = ch_a,
                        isMelee = meleeA

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



        inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorldSystem.GetOutputDependency());
        inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.GetOutputDependency());
        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;
        var collisionJob = new CollisionJob
        {
            physicsWorld = physicsWorld,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
            triggerGroup = GetComponentDataFromEntity<TriggerComponent>(true),
            meleeGroup = GetComponentDataFromEntity<MeleeComponent>(true),
        };
        JobHandle collisionHandle = collisionJob.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);
        collisionHandle.Complete();
        return collisionHandle;
    } // OnUpdate




} // System


