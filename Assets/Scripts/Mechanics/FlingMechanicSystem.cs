using System.Diagnostics;
using Rewired;
using SandBox.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace SandBox.Player
{
    //[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    //[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateAfter(typeof(PlayerRotateSystem))]

    public class FlingMechanicSystem : SystemBase
    {


        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            


            Entities.WithoutBurst().WithAll<FlingMechanicComponent, PlayerComponent>().ForEach(
                (
                    in DamageComponent damageComponent,
                    in Impulse impulse) =>
                {
                    if (damageComponent.DamageReceived <= math.EPSILON) return;
                    impulse.impulseSourceHitReceived.GenerateImpulse();

                }
            ).Run();



            Entities.WithoutBurst().WithAll<FlingMechanicComponent, PlayerComponent>().ForEach(
                (
                    in ScoreComponent score,
                    in Impulse impulse) =>
                {
                    if (score.pointsScored == false) return;
                    impulse.impulseSourceHitLanded.GenerateImpulse();

                }
            ).Run();


            var scoreGroup = GetComponentDataFromEntity<ScoreComponent>(false);




            Entities.WithoutBurst().WithAll<PlayerComponent>().WithNone<Pause>().ForEach(
                (
                    Entity e,
                    ref PhysicsVelocity pv,
                    ref FlingMechanicComponent flingMechanic,
                    in InputController inputController,
                    in PlayerMove playerMove,
                    in FlingMechanicComponentAuthoring flingMechanicComponentAuthoring

                ) =>
                {


                    bool hasScore = HasComponent<ScoreComponent>(e);


                    float3 forward = playerMove.transform.forward;
                    forward = math.normalize(forward);
                
                    flingMechanic.timeSinceCausingDamage += Time.DeltaTime;

                    if (flingMechanic.inFling == true && flingMechanic.inFlingTime >= flingMechanic.inFlingMaxTime)
                    {

                        pv.Linear = Vector3.zero;
                        flingMechanic.inFling = false;
                        flingMechanic.vulnerable = true;

                        if(flingMechanic.shotLanded == true)
                        {
                            flingMechanic.lastShotConnected = true;
                            flingMechanic.vulnerableMaxTimeGame = flingMechanic.vulnerableMaxTimeGame * .8f;//less time that can not shoot as streak grows
                        }
                        else
                        {
                            flingMechanic.lastShotConnected = false;//reset
                            flingMechanic.vulnerableMaxTimeGame = flingMechanic.
                                vulnerableMaxTime;//reset after streak broken
                        }

                        if (hasScore)
                        {
                            var score = scoreGroup[e];
                            if(flingMechanic.lastShotConnected == false && score.combo == 0)
                            {
                                score.streak = 0;
                            }
                            score.combo = 0;
                            scoreGroup[e] = score;
                        }



                        flingMechanic.shotLanded = false;//reset 

                        if (flingMechanicComponentAuthoring.inFlingParticleSystem)
                        {
                            flingMechanicComponentAuthoring.inFlingParticleSystem.Stop(true);
                        }

                        if (flingMechanicComponentAuthoring.vulnerableParticleSystem)
                        {
                            flingMechanicComponentAuthoring.vulnerableParticleSystem.Play(true);
                        }


                        //flingMechanic.inFlingTime = 0;
                    }
                    else if (flingMechanic.inFling == true && flingMechanic.inFlingTime < flingMechanic.inFlingMaxTime)
                    {
                        //pv.Linear = forward * flingMechanic.force;
                        flingMechanic.inFlingTime = flingMechanic.inFlingTime + Time.DeltaTime;
                    }
                    else if (inputController.leftTriggerPressed == true && flingMechanic.vulnerable == false)
                    {
                        flingMechanic.shotLanded = false;

                        pv.Linear = forward * flingMechanic.force;
                        //Debug.Log("pv ");
                        flingMechanic.inFling = true;
                        flingMechanic.inFlingTime = 0;
                        if (flingMechanicComponentAuthoring.inFlingParticleSystem)
                        {
                            flingMechanicComponentAuthoring.inFlingParticleSystem.Play(true);
                        }
                        if (flingMechanicComponentAuthoring.flingAudioSource)
                        {
                            flingMechanicComponentAuthoring.flingAudioSource.PlayOneShot(flingMechanicComponentAuthoring.flingAudioClip);
                        }

                    }
                    else if(flingMechanic.vulnerable == true && flingMechanic.vulnerableTime < flingMechanic.vulnerableMaxTimeGame)
                    {
                        flingMechanic.vulnerableTime = flingMechanic.vulnerableTime + Time.DeltaTime;
                    }
                    else
                    {
                        flingMechanic.vulnerable = false;
                        flingMechanic.vulnerableTime = 0;
                        if (flingMechanicComponentAuthoring.vulnerableParticleSystem)
                        {
                            flingMechanicComponentAuthoring.vulnerableParticleSystem.Stop(true);
                        }

                    }
                }


            ).Run();


        }
    }


}