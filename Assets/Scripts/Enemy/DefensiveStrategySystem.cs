



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
public class DefensiveStrategySystem : SystemBase
{



    protected override void OnUpdate()
    {

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);





        //var triggerGroup = GetComponentDataFromEntity<TriggerComponent>(false);
        EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Persistent);
        //NativeArray<TriggerComponent> triggerGroup = triggerQuery.ToComponentDataArray<TriggerComponent>(Allocator.Persistent);
        //NativeArray<AmmoComponent> ammoGroup = triggerQuery.ToComponentDataArray<AmmoComponent>(Allocator.Persistent);
        int players = playerEntities.Length;

        Entities.WithoutBurst().WithAny<EnemyComponent>().ForEach((ref DefensiveStrategyComponent defensiveStrategyComponent, in Entity enemyE,
                in Translation enemyTranslation) =>
        {

            for (int i = 0; i < players; i++)
            {
                var playerE = playerEntities[i];
                if (defensiveStrategyComponent.currentRole == DefensiveRoles.Chase)
                {
                    //Debug.Log("player entity " + playerE + " chased by enemy entity " + enemyE);
                    if(defensiveStrategyComponent.currentRoleTimer < defensiveStrategyComponent.currentRoleMaxTime)
                    {
                        defensiveStrategyComponent.currentRoleTimer += Time.DeltaTime;
                    }
                    else
                    {
                        defensiveStrategyComponent.currentRole = DefensiveRoles.None;
                        defensiveStrategyComponent.currentRoleTimer = 0;
                    }
                }
            }



        }

        ).Run();






        ecb.Playback(EntityManager);
        ecb.Dispose();
        //triggerGroup.Dispose();
        //ammoGroup.Dispose();
        playerEntities.Dispose();
        //return default;
    }



}





