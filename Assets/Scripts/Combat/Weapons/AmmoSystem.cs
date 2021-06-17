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
                        Debug.Log(" last shot " + score.lastShotConnected);
                        if (score.lastShotConnected == false) score.streak = 0;
                        //score.lastShotConnected = false;
                        scoreGroup[shooter] = score;
                        //also can do score counter of number of connects before ammo destroyed here

                    }




                    //var bullet = EntityManager.GetComponentData<AmmoComponent>(entity);
                    //bullet.AmmoDead = true;
                    //bullet.AmmoTimeCounter = 0;
                    //EntityManager.SetComponentData(entity, bullet);
                    ecb.DestroyEntity(entity);
                    //Debug.Log("amo dead");
                }
            }
        ).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        //return default;
    }



}



