using SandBox.Player;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

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
    public float timer;

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

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateBefore(typeof(PlayerMoveSystem))]


public class CollisionSystem : SystemBase
{
    //EndSimulationEntityCommandBufferSystem m_ecbSystem;
    EndFixedStepSimulationEntityCommandBufferSystem m_ecbSystem;
    BuildPhysicsWorld buildPhysicsWorldSystem;
    StepPhysicsWorld stepPhysicsWorld;
    EndFramePhysicsSystem endFramePhysicsSystem;

    protected override void OnCreate()
    {
        //m_ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_ecbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        buildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

    }

    protected override void OnUpdate()
    {

        var inputDeps = new JobHandle();

        inputDeps = JobHandle.CombineDependencies(inputDeps, buildPhysicsWorldSystem.GetOutputDependency());
        inputDeps = JobHandle.CombineDependencies(inputDeps, stepPhysicsWorld.GetOutputDependency());
        var physicsWorld = buildPhysicsWorldSystem.PhysicsWorld;
        var collisionJob = new CollisionJob
        {
            physicsWorld = physicsWorld,
            ecb = m_ecbSystem.CreateCommandBuffer(),
            triggerGroup = GetComponentDataFromEntity<TriggerComponent>(true),
            healthGroup = GetComponentDataFromEntity<HealthComponent>(true),
            ammoGroup = GetComponentDataFromEntity<AmmoComponent>(false)
        };
        JobHandle collisionHandle = collisionJob.Schedule(stepPhysicsWorld.Simulation, ref physicsWorld, inputDeps);
        //m_ecbSystem.AddJobHandleForProducer(this.Dependency);

        //m_ecbSystem.AddJobHandleForProducer(collisionHandle);
        collisionHandle.Complete();




        //return collisionHandle;
    } // OnUpdate


    struct CollisionJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public ComponentDataFromEntity<TriggerComponent> triggerGroup;
        [ReadOnly] public ComponentDataFromEntity<HealthComponent> healthGroup;
        public ComponentDataFromEntity<AmmoComponent> ammoGroup;
        public EntityCommandBuffer ecb;
        public void Execute(CollisionEvent ev) // this is never called
        {
            Entity a = ev.EntityA;
            Entity b = ev.EntityB;
            //Entity a =   physicsWorld.Bodies[ev.BodyIndexA].Entity;
            //Entity b = physicsWorld.Bodies[ev.BodyIndexB].Entity;
            //Debug.Log("ena " + a + " enb " + b);

            if (triggerGroup.HasComponent(a) == false || triggerGroup.HasComponent(b) == false) return;
            var triggerComponent_a = triggerGroup[a];
            var triggerComponent_b = triggerGroup[b];

            Entity ch_a = triggerComponent_a.ParentEntity;
            Entity ch_b = triggerComponent_b.ParentEntity;
            int type_a = triggerComponent_a.Type;
            int type_b = triggerComponent_b.Type;

            //Debug.Log("tya " + type_a + " tyb  " + type_b);


            if (ch_a == ch_b) return;////?????






            bool alwaysDamageA = false;
            if (healthGroup.HasComponent(ch_a) == true)
            {
                var healthComponent_a = healthGroup[ch_a];
                alwaysDamageA = healthComponent_a.AlwaysDamage;
            }

            bool alwaysDamageB = false;
            if (healthGroup.HasComponent(ch_b) == true)
            {
                var healthComponent_b = healthGroup[ch_b];
                alwaysDamageB = healthComponent_b.AlwaysDamage;//regardless of type trigger
            }


            if (triggerComponent_a.Type == (int)TriggerType.Ground ||
                triggerComponent_b.Type == (int)TriggerType.Ground)
            {
                return;
            }

            //Debug.Log("ta " + triggerComponent_a.Type);
            //Debug.Log("tb " + triggerComponent_b.Type);
            //Debug.Log("always a " + alwaysDamageA + " always b " + alwaysDamageB + " cha " + ch_a + " chb " + ch_b);


            bool punchingA = false;
            bool punchingB = false;
            if (type_a == (int)TriggerType.Body || type_a == (int)TriggerType.Base || type_a == (int)TriggerType.Head)
            {
                punchingB = true;
            }
            else if (type_b == (int)TriggerType.Body || type_b == (int)TriggerType.Base || type_b == (int)TriggerType.Head)

            {
                punchingA = true;
            }


            //if punching A or B is true then we dont skip eventhough type a = type b 
            if (type_a == type_b && punchingA == false && punchingB == false && alwaysDamageA == false && alwaysDamageB == false) return;



            bool meleeA = (type_b == (int)TriggerType.Base || type_b == (int)TriggerType.Head) &&
    (type_a == (int)TriggerType.Melee);//doubt this will be needed

            bool meleeB = (type_a == (int)TriggerType.Base || type_a == (int)TriggerType.Head) &&
    (type_b == (int)TriggerType.Melee);//doubt this will be needed




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

            bool ammoBlockedA = (type_b == (int)TriggerType.Blocks) &&
                         (type_a == (int)TriggerType.Ammo);

            bool ammoBlockedB = (type_a == (int)TriggerType.Blocks) &&
                         (type_b == (int)TriggerType.Ammo);

            //Debug.Log("aa " + ammoA + " ab " + ammoB);
            if (ammoBlockedA)
            {
                AmmoComponent ammoComponent = ammoGroup[triggerComponent_a.Entity];
                ammoComponent.Charged = true;
                //Debug.Log("charged a");
                ammoGroup[triggerComponent_a.Entity] = ammoComponent;
            }
            if (ammoBlockedB)
            {
                AmmoComponent ammoComponent = ammoGroup[triggerComponent_b.Entity];
                ammoComponent.Charged = true;
                //Debug.Log("charged b");
                ammoGroup[triggerComponent_b.Entity] = ammoComponent;
            }





            if (ammoA)
            {
                //Debug.Log("aa " + ammoA + " pe " + triggerComponent_b.Type + "  ce " + ch_b);
                //coll component part other always ammo ?
                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_b.Type,
                        Part_other_entity = triggerComponent_a.Type,
                        Character_entity = triggerComponent_b.ParentEntity,//actor hit by ammo
                        Character_other_entity = triggerComponent_a.Entity,
                        isMelee = meleeA
                    };
                ecb.AddComponent(triggerComponent_a.ParentEntity, collisionComponent);
            }
            else if (ammoB)
            {
                //Debug.Log("ab " + ammoB + " pe " + triggerComponent_a.Type + "  ce " + ch_a);

                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_a.Type,
                        Part_other_entity = triggerComponent_b.Type,
                        Character_entity = triggerComponent_a.ParentEntity,
                        Character_other_entity = triggerComponent_b.Entity,
                        isMelee = meleeB

                    };

                ecb.AddComponent(triggerComponent_b.ParentEntity, collisionComponent);
            }
            else if (punchingA ||  meleeA || alwaysDamageA && !ammoA && !ammoB)
            {
                //Debug.Log("c b " + ch_b + " c a " + ch_a);
                //Debug.Log("always a " + alwaysDamageA + " always b " + alwaysDamageB + " cha " + ch_a + " chb " + ch_b);

                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_a.Type,
                        Part_other_entity = triggerComponent_b.Type,
                        Character_entity = ch_a,
                        Character_other_entity = ch_b,
                        isMelee = meleeA
                    };
                ecb.AddComponent(ch_a, collisionComponent);
            }
            else if (punchingB  || meleeB || alwaysDamageB && !ammoA && !ammoB)
            {
                //Debug.Log("always a " + alwaysDamageA + " always b " + alwaysDamageB + " cha " + ch_a + " chb " + ch_b);

                CollisionComponent collisionComponent =
                    new CollisionComponent()
                    {
                        Part_entity = triggerComponent_b.Type,
                        Part_other_entity = triggerComponent_a.Type,
                        Character_entity = ch_b,
                        Character_other_entity = ch_a,
                        isMelee = meleeB

                    };
                ecb.AddComponent(ch_b, collisionComponent);
            }



        }
    }








} // System


