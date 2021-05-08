using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;






public class BasicLoserSystem : SystemBase
{

    protected override void OnUpdate()
    {
        if (LevelManager.instance.gameResult == GameResult.Loser) return;


        bool loser = false;


        Entities.WithAll<PlayerComponent>().WithoutBurst().ForEach
        (
            (in DeadComponent dead) =>
            {
                if (dead.isDead == true)
                {
                    loser = true;
                    Debug.Log("loser");
                }
            }
        ).Run();

   
        if (loser == false) return;

        LevelManager.instance.endGame = true;
        LevelManager.instance.gameResult = GameResult.Loser;

    }
}



[UpdateAfter(typeof(BasicWinnerSystem))]

public class EndGameSystem : SystemBase
{

    StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }


    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        if (LevelManager.instance.gameResult == GameResult.Winner || LevelManager.instance.gameResult == GameResult.Loser)
        {




            Entities.WithoutBurst().WithAny<PlayerComponent, EnemyComponent>().ForEach
            ((in Entity e) =>
                {
            
                    if (HasComponent<PhysicsVelocity>(e))
                    {
                        ecb.RemoveComponent<PhysicsVelocity>(e);
                    }

                    if (HasComponent<Pause>(e))
                    {
                        ecb.RemoveComponent<Pause>(e);
                    }
                }
            ).Run();


        }

        ecb.Playback(EntityManager);
        ecb.Dispose();


    }
}
