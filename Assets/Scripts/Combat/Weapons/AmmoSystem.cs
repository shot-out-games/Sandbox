using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;




[UpdateAfter(typeof(GunAmmoHandlerSystem))]//gunammohandler makes the  bullet first
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class AmmoSystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        float dt = Time.fixedDeltaTime;//bullet duration

        var scoreGroup = GetComponentDataFromEntity<ScoreComponent>(false);



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

        //return default;
    }



}



