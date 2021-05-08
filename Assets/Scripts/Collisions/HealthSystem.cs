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

        bool anyEnemyDamaged = false;
        bool anyPlayerDamaged = false;
        Entities.WithoutBurst().ForEach((ref DeadComponent deadComponent,
            ref HealthComponent healthComponent, ref DamageComponent damageComponent,
            ref RatingsComponent ratingsComponent,
            in Entity entity) =>
            {
                healthComponent.ShowDamage = false;
                if (EntityManager.HasComponent(entity, typeof(EnemyComponent)))
                {
                    if (EntityManager.GetComponentData<EnemyComponent>(entity).invincible == true)
                    {
                        damageComponent.DamageReceived = 0;
                    }
                }

                if (damageComponent.DamageReceived > healthComponent.ShowDamageMin)
                {
                    healthComponent.ShowDamage = true;
                }

                healthComponent.TotalDamageReceived += damageComponent.DamageReceived;
                ecb.RemoveComponent<DamageComponent>(entity);
                var dead = EntityManager.GetComponentData<DeadComponent>(entity);
                if (damageComponent.DamageReceived > 0)
                {
                    if (HasComponent<EnemyComponent>(entity) == true)
                    {
                        anyEnemyDamaged = true;
                    }
                    else if (HasComponent<PlayerComponent>(entity) == true)
                    {
                        anyPlayerDamaged = true;
                    }
                }



                if (healthComponent.TotalDamageReceived >= ratingsComponent.maxHealth && dead.isDead == false)
                {
                    dead.dieLevel = LevelManager.instance.currentLevelCompleted;
                    dead.isDying = true;
                    dead.isDead = true;
                    dead.playDeadEffects = true;
                    ecb.SetComponent(entity, dead);
                }

            }
        ).Run();

        if(anyEnemyDamaged == false && anyPlayerDamaged == false) return;

        Entities.WithoutBurst(). ForEach((HealthBar healthUI, in HealthComponent healthComponent, in DamageComponent damage) =>
        {
            if (healthComponent.ShowText3D == ShowText3D.hitScore && healthComponent.ShowDamage == true)
            {
                healthUI.ShowText3dValue((int)damage.ScorePointsReceived);
            }
            else if (healthComponent.ShowText3D == ShowText3D.hitDamage && healthComponent.ShowDamage == true)
            {
                healthUI.ShowText3dValue((int)damage.DamageReceived);
            }
            healthUI.HealthChange();

        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();



    }



}




