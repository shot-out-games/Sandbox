using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class NdeMechanicSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //var ecb = ecbSystem.CreateCommandBuffer();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        var gunGroup = GetComponentDataFromEntity<GunComponent>(true);


        Entities.ForEach(
            (
                Entity e,
                ref NdeMechanicComponent ndeMechanic,
                ref RatingsComponent ratings,
                ref PlayerJumpComponent playerJump,
                in HealthComponent health



            ) =>
            {

                float pct = health.TotalDamageReceived / ratings.maxHealth;

                ndeMechanic.multiplier = pct + .50f;//temp


                float f1 = pct / 2;
                float f2 = pct  * 2.2f;
                ratings.gameSpeed = ratings.speed * (.80f + f1);
                playerJump.gameStartJumpGravityForce = playerJump.startJumpGravityForce * (pct + .50f);

                bool hasGun = gunGroup.HasComponent(e);
                if (hasGun)
                {
                    var gun = gunGroup[e];
                    gun.gameStrength = gun.Strength * ndeMechanic.multiplier;
                    gun.gameRate = gun.Rate * ndeMechanic.multiplier;
                    gun.gameDamage = gun.Damage * ndeMechanic.multiplier;
                    //EntityManager.SetComponentData(e, gun);
                    ecb.SetComponent(e, gun);
                }

            }


        ).Run();

        


        ecb.Playback(EntityManager);
        //ecb.Dispose();


        //ecbSystem.AddJobHandleForProducer(Dependency);






    }
}



//using Unity.Entities;

//namespace EconSim
//{
//    public class UpdateDesiredStateSystem : SystemBase
//    {
//        EndSimulationEntityCommandBufferSystem ecbSystem;

//        protected override void OnCreate()
//        {
//            base.OnCreate();
//            ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//        }

//        protected override void OnUpdate()
//        {
//            var ecb = ecbSystem.CreateCommandBuffer().ToConcurrent();

//            Entities
//                .ForEach((Entity entity, int entityInQueryIndex, in DesiredStateData desiredState, in TimeRemainingData timeRemaining) =>
//                {
//                    // Check if entity has the IsActiveFlag component
//                    bool isActive = false; //???

//                    // Entity wants to be active
//                    if (desiredState.value)
//                    {
//                        if (!isActive)
//                        {
//                            // add an entry to the ECB to add the IsActiveFlag component
//                            ecb.AddComponent<IsActiveFlag>(entityInQueryIndex, entity);
//                        }
//                    }
//                    // Entity wants to be disabled
//                    else
//                    
//                        if (isActive && timeRemaining.value <= 0)
//                        {
//                            ecb.RemoveComponent<IsActiveFlag>(entityInQueryIndex, entity);
//                        }
//                    }

//                }).ScheduleParallel();

//            ecbSystem.AddJobHandleForProducer(Dependency);
//        }
//    }
//}