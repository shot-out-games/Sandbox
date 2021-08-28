using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
//[UpdateInGroup(typeof(TransformSystemGroup))]
//[UpdateAfter(typeof(EndFrameLocalToParentSystem))]
//[UpdateBefore(typeof(SynchronizeGameObjectTransformsWithTriggerEntities))]
//[UpdateBefore(typeof(EndFrameTRSToLocalToWorldSystem))]

public class FinalIkSystem : SystemBase
{
    protected override void OnUpdate()
    {


        Entities.WithoutBurst().ForEach((PlayerCombat playerCombat) =>
        {

            playerCombat.LateUpdateSystem();


        }).Run();

        Entities.WithoutBurst().ForEach((EnemyMelee  enemyMelee) =>
        {

            enemyMelee.LateUpdateSystem();


        }).Run();
    }
}
