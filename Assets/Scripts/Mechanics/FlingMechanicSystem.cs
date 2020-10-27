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


            Entities.WithoutBurst().WithAll<FlingMechanicComponent>().ForEach(
                (
                    in Pause pause,
                    in DamageComponent damageComponent,
                    in Impulse impulse) =>
                {
                    if (damageComponent.DamageReceived <= math.EPSILON || pause.value == 1) return;
                    impulse.impulseSourceHitReceived.GenerateImpulse();

                }
            ).Run();



            Entities.WithoutBurst().WithAll<FlingMechanicComponent>().ForEach(
                (
                    in Pause pause,
                    in ScoreComponent score,
                    in Impulse impulse) =>
                {
                    if (score.pointsScored == false || pause.value == 1) return;
                    impulse.impulseSourceHitLanded.GenerateImpulse();

                }
            ).Run();





            Entities.WithoutBurst().ForEach(
                (
                    Entity e,
                    ref PhysicsVelocity pv,
                    ref FlingMechanicComponent flingMechanic,
                    in InputController inputController,
                    in PlayerMove playerMove,
                    in FlingMechanicComponentAuthoring flingMechanicComponentAuthoring

                ) =>
                {

                    float3 forward = playerMove.transform.forward;
                    forward = math.normalize(forward);
                    //Debug.Log("fw " + forward);

                    if (inputController.leftTriggerPressed == true)
                    {
                        Debug.Log("press");
                    }



                    if (flingMechanic.inFling == true && flingMechanic.inFlingTime >= flingMechanic.inFlingMaxTime)
                    {
                        pv.Linear = Vector3.zero;
                        flingMechanic.inFling = false;
                        flingMechanic.vulnerable = true;
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
                    else if(flingMechanic.vulnerable == true && flingMechanic.vulnerableTime < flingMechanic.vulnerableMaxTime)
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