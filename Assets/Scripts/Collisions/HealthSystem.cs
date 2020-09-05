using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.Rendering;

public class HealthSystem : JobComponentSystem
{

    private EndSimulationEntityCommandBufferSystem ecbSystem;





    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.WithoutBurst().ForEach((ref DeadComponent deadComponent,
            ref HealthComponent healthComponent, ref DamageComponent damageComponent,
            ref RatingsComponent ratingsComponent,
            in Entity entity) =>
            {
                if (EntityManager.HasComponent(entity, typeof(EnemyComponent)))
                {
                    if(EntityManager.GetComponentData<EnemyComponent>(entity).invincible == true)
                    {
                        damageComponent.DamageReceived = 0;
                        damageComponent.DamageLanded = 0;
                    }
                }



                //healthComponent.TotalDamageReceived += damageComponent.DamageReceived + damageComponent.DamageLanded; //kenney
                healthComponent.TotalDamageReceived += damageComponent.DamageReceived;
                //healthComponent.TotalDamageLanded += damageComponent.DamageLanded;
                //Debug.Log("dam  " + damageComponent.DamageReceived);
                ecb.RemoveComponent<DamageComponent>(entity);

                var dead = EntityManager.GetComponentData<DeadComponent>(entity);
                if (healthComponent.TotalDamageReceived >= ratingsComponent.maxHealth && dead.isDead == false && dead.justDead == false)
                {
                    dead.isDead = true;
                    dead.justDead= true;
                    ecb.SetComponent(entity, dead);//tag player or enemy
                }

            }
        ).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();


        return default;


    }



}




//public class HealthSystem : JobComponentSystem
//{

//    private EndSimulationEntityCommandBufferSystem barrier;


//    protected override void OnCreate()
//    {
//        barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

//    }


//    private struct HealthJob : IJobForEachWithEntity<HealthComponent, DamageComponent, RatingsComponent >
//    {



//        public EntityCommandBuffer.Concurrent Ecb;
//        [ReadOnly] public ComponentDataFromEntity<DeadComponent> Dead;

//        public void Execute(
//            Entity entity,
//            int index,
//            ref HealthComponent healthComponent,
//            ref DamageComponent damageComponent,
//            ref RatingsComponent ratingsComponent

//            )
//        {
//            healthComponent.TotalDamageReceived += damageComponent.DamageReceived;
//            healthComponent.TotalDamageLanded += damageComponent.DamageLanded;
//            Debug.Log("damage received ");
//            Debug.Log("damage landed ");
//            Ecb.RemoveComponent<DamageComponent>(index, entity);

//            if (healthComponent.TotalDamageReceived >= ratingsComponent.maxHealth && !Dead.Exists(entity))
//            {
//                Ecb.AddComponent(index, entity, new DeadComponent { tag = ratingsComponent.tag });//tag player or enemy
//            }

//        }
//    }

//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        var job = new HealthJob
//        {
//            Ecb = barrier.CreateCommandBuffer().ToConcurrent(),
//            Dead = GetComponentDataFromEntity<DeadComponent>()
//        };
//        inputDeps = job.Schedule(this, inputDeps);
//        barrier.AddJobHandleForProducer(inputDeps);
//        return inputDeps;
//    }



//}



