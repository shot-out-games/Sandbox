using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class LevelUpMechanicSystem : SystemBase
{

    //[NativeDisableParallelForRestriction] 
    //EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        //ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //var ecb = ecbSystem.CreateCommandBuffer();
        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        //var gunGroup = GetComponentDataFromEntity<GunComponent>(true);


        Entities.ForEach(
            (
                Entity e,
                ref ControlBarComponent controlBar,
                ref LevelUpMechanicComponent levelUpMechanic,
                ref RatingsComponent ratings,
                //ref PlayerJumpComponent playerJump,
                in HealthComponent health,
                in SkillTreeComponent skillTreeComponent

            ) =>
            {
                int pointsNeeded = skillTreeComponent.PointsNextLevel * skillTreeComponent.CurrentLevel;


                float pct = skillTreeComponent.CurrentLevelXp / (float)pointsNeeded;
                //ndeMechanic.multiplier = .5f;
                controlBar.value = pct;
                    //Debug.Log("ctrl " + controlBar.value);
                //float f1 = pct * .5f;
                //float f2 = pct * .66f;

                //ratings.gameSpeed = ratings.speed  + skillTreeComponent.CurrentLevel;
                //playerJump.gameStartJumpGravityForce = playerJump.startJumpGravityForce * (f1 + .50f);


            }


        ).ScheduleParallel();

        //Entities.WithAll<ControlBarComponent, NdeMechanicComponent>().ForEach
        //(
        //    (
        //        ref GunComponent gun,
        //        in HealthComponent health,
        //        in RatingsComponent ratings
        //        ) =>
        //    {

        //        float pct = health.TotalDamageReceived / ratings.maxHealth;
        //        float f1 = pct * .5f;
        //        //float f2 = pct * .66f;


        //        //bool hasGun = gunGroup.HasComponent(e);
        //        //bool hasGun = HasComponent<GunComponent>(e);
        //        //if (hasGun)
        //        //{
        //        // var gun = gunGroup[e];
        //        gun.gameStrength = gun.Strength * (.50f + f1);
        //        gun.gameRate =  gun.Rate + gun.Rate * (1 - pct);
        //        gun.gameDamage = gun.Damage * (.50f + f1);

        //        //ecb.SetComponent(entityInQueryIndex, e, gun);
        //        //ecb.SetComponent(e, gun);
        //        //}

        //    }

        //).ScheduleParallel();



        //ecbSystem.AddJobHandleForProducer(Dependency);

        //ecb.Playback(EntityManager);
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