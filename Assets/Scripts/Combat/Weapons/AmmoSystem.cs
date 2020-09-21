using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;




//[UpdateAfter(typeof(GunAmmoHandlerSystem))]//gunammohandler makes the  bullet first
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class AmmoSystem : JobComponentSystem
{



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        float dt = Time.fixedDeltaTime;//bullet duration


        Entities.WithoutBurst().ForEach(
            (ref AmmoComponent ammo, in Entity entity) =>
            {


                //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                ammo.AmmoTimeCounter += dt;
                if (ammo.AmmoTimeCounter > ammo.AmmoTime)
                {
                    var bullet = EntityManager.GetComponentData<AmmoComponent>(entity);
                    bullet.AmmoDead = true;
                    bullet.AmmoTimeCounter = 0;
                    EntityManager.SetComponentData(entity, bullet);
                    //EntityManager.DestroyEntity(entity);
                    //Debug.Log("amo dead");
                }
            }
        ).Run();


        return default;
    }



}



