using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class CubeMechanicSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach
        ((ref CubeComponent cubeComponent, ref Translation translation, ref PhysicsVelocity pv, in Entity e
        ) =>
        {
            if (HasComponent<DeadComponent>(e))
            {
                ecb.RemoveComponent<DeadComponent>(e);
            }


            if (cubeComponent.dead == true)
            {
                pv.Linear += new float3(0, 5,  0);
            }


        }).Run();


        bool rightTriggerDown = true;
        int cubesDestroyed = 0;
        int cubesMax = 0;
        bool destroyed = false;

        Entities.WithoutBurst().WithStructuralChanges().ForEach
        ((CubeSpawner cube) =>
        {
            cubesMax = cube.maxCubes;
            //cube.ReadInput();
            rightTriggerDown = cube.rightTriggerDown;
            if (rightTriggerDown == true & destroyed == false && cube.cubesDestroyed < cube.spawned)
            {
                cube.cubesDestroyed += 1;
                cubesDestroyed = cube.cubesDestroyed;
                destroyed = true;
            }

        }).Run();

        if (destroyed == false) return;

        if(cubesDestroyed > cubesMax) return;

        bool bonus = cubesDestroyed == cubesMax;

        bool entityDestroyed = false;

        Entities.WithoutBurst().ForEach
        ((ref CubeComponent cubeComponent, ref Translation translation,  ref PhysicsVelocity pv,  in Entity e
            ) =>
        {
            if (entityDestroyed == false && cubeComponent.dead == false)
            {
                entityDestroyed = true;
                cubeComponent.dead = true;
                //ecb.DestroyEntity(e);
                //translation.Value.y  = -50f;
                //pv.Linear = float3.zero;
            }


        }).Run();


        Entities.WithoutBurst().WithStructuralChanges().ForEach
        ((CubeSpawner cube) =>
        {
            if (cube.bonusEarned == true)
            {
                bonus = false;
                return;
            }

            if (bonus == true) cube.bonusEarned = true;


        }).Run();


        Entities.WithoutBurst().ForEach
        ((ref ScoreComponent score, ref FlingMechanicComponent flingMechanic, in ScoreComponentAuthoring scoreComponentAuthoring) =>
        {
            int add = 50 * (LevelManager.instance.currentLevelCompleted + 1);
            if (bonus == true)
            {
                add = add +  add * cubesMax;
            }

            score.score += add;
            flingMechanic.vulnerableMaxTimeGame = flingMechanic.vulnerableMaxTimeGame + .2f;
            flingMechanic.vulnerableMaxTime = flingMechanic.vulnerableMaxTime + .05f;

            scoreComponentAuthoring.ShowLabelScore(score.score);


        }).Run();





        ecb.Playback(EntityManager);
        ecb.Dispose();

    }
}
