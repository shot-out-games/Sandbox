



using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;




//[UpdateAfter(typeof(GunAmmoHandlerSystem))]//gunammohandler makes the  bullet first
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class AmmoSystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        float dt = Time.fixedDeltaTime;//bullet duration

        var scoreGroup = GetComponentDataFromEntity<ScoreComponent>(false);

        
        //var triggerGroup = GetComponentDataFromEntity<TriggerComponent>(false);
        EntityQuery triggerQuery = GetEntityQuery(ComponentType.ReadOnly<TriggerComponent>(),
            ComponentType.ReadOnly<AmmoComponent>());
        NativeArray<Entity> triggerEntities = triggerQuery.ToEntityArray(Allocator.Persistent);
        NativeArray<TriggerComponent> triggerGroup = triggerQuery.ToComponentDataArray<TriggerComponent>(Allocator.Persistent);
        NativeArray<AmmoComponent> ammoGroup = triggerQuery.ToComponentDataArray<AmmoComponent>(Allocator.Persistent);


        Entities.WithoutBurst().WithAny<EnemyComponent>().ForEach((ref DefensiveStrategyComponent defensiveStrategy, in Translation enemyTranslation) =>
        {

            for (int i = 0; i < ammoGroup.Length; i++)
            {
                TriggerComponent trigger = triggerGroup[i];
                var ammoE = triggerEntities[i];
                var triggerTranslation = GetComponent<Translation>(triggerEntities[i]);
                var shooter = trigger.ParentEntity;

                bool isEnemy = HasComponent<EnemyComponent>(shooter);
                if (isEnemy) return;

                var playerTranslation = GetComponent<Translation>(shooter);//not used

                float distance = math.distance(triggerTranslation.Value, enemyTranslation.Value);
                //Debug.Log("dt " + (int)distance);
                if (distance < defensiveStrategy.breakRouteVisionDistance && defensiveStrategy.breakRoute == true && ammoE != defensiveStrategy.closeBulletEntity)
                {
                    defensiveStrategy.closeBulletEntity = ammoE;
                    defensiveStrategy.currentRole = DefensiveRoles.Chase;
                    defensiveStrategy.currentRoleTimer = 0;
                }

            }



        }

        ).Run();





        Entities.WithoutBurst().ForEach(
            (ref AmmoComponent ammo, in Entity entity) =>
            {


                //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                ammo.AmmoTimeCounter += dt;
                if (ammo.AmmoTimeCounter > ammo.AmmoTime)
                {

                    Entity shooter = EntityManager.GetComponentData<TriggerComponent>(entity)
                        .ParentEntity;

                    if (HasComponent<ScoreComponent>(shooter))
                    {
                        var score = scoreGroup[shooter];
                        //Debug.Log(" last shot " + score.lastShotConnected);
                        if (score.lastShotConnected == false) score.streak = 0;
                        score.combo = 0;
                        scoreGroup[shooter] = score;
                        ammo.ammoHits = 0;

                    }
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    if (ammo.DamageCausedPreviously) ammo.frameSkipCounter = ammo.frameSkipCounter + 1;

                    Entity shooter = EntityManager.GetComponentData<TriggerComponent>(entity)
                        .ParentEntity;

                    if (HasComponent<ScoreComponent>(shooter))
                    {
                        var score = scoreGroup[shooter];


                        if (score.pointsScored && score.scoringAmmoEntity == entity)
                        {
                            ammo.ammoHits += 1;
                            Debug.Log("ammo hits1 " + ammo.ammoHits);
                            score.combo = ammo.ammoHits;
                            ammo.AmmoTime += ammo.comboTimeAdd;
                        }

                        scoreGroup[shooter] = score;

                        //also can do score counter of number of connects before ammo destroyed here
                    }

                }




            }
        ).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        triggerGroup.Dispose();
        ammoGroup.Dispose();
        triggerEntities.Dispose();
        //return default;
    }



}





