using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.Rendering;

public class HealthSystem : SystemBase
{

    private EndSimulationEntityCommandBufferSystem ecbSystem;





    protected override void OnUpdate()
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
                    }
                }

                healthComponent.TotalDamageReceived += damageComponent.DamageReceived;
                ecb.RemoveComponent<DamageComponent>(entity);
                var dead = EntityManager.GetComponentData<DeadComponent>(entity);
                if (healthComponent.TotalDamageReceived >= ratingsComponent.maxHealth && dead.isDead == false && dead.justDead == false)
                {
                    dead.dieLevel = LevelManager.instance.currentLevelCompleted;
                    dead.isDead = true;
                    dead.justDead= true;
                    ecb.SetComponent(entity, dead);
                }

            }
        ).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();



    }



}




