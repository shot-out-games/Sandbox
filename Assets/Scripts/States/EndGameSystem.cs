using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

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

        if (LevelManager.instance.gameResult == GameResult.Winner)
        {


            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


            Entities.WithoutBurst().ForEach
            ((AudioSource AudioSource) =>
                {
                    if (AudioSource)
                    {
                        if (AudioSource.isPlaying)
                        {
                            AudioSource.Stop();
                        }
                    }
                }
            ).Run();

            Entities.WithoutBurst().WithAny<PlayerComponent, EnemyComponent>().ForEach
            ((in Entity e) =>
                {
                    if (HasComponent<PlayerComponent>(e))
                    {
                        //ecb.RemoveComponent<PlayerComponent>(e);
                    }
                    else if (HasComponent<EnemyComponent>(e))
                    {
                        //ecb.RemoveComponent<EnemyComponent>(e);
                    }

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




            ecb.Playback(EntityManager);
            ecb.Dispose();



        }

    }
}
