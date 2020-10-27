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

            Entities.WithoutBurst().ForEach(
                (
                    Entity e,
                    ref PhysicsVelocity pv,
                    ref FlingMechanicComponent flingMechanic,
                    in InputController inputController,
                    in PlayerMove playerMove

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
                        //flingMechanic.inFlingTime = 0;
                    }
                    else if (flingMechanic.inFling == true && flingMechanic.inFlingTime < flingMechanic.inFlingMaxTime)
                    {
                        //pv.Linear = forward * flingMechanic.force;
                        flingMechanic.inFlingTime = flingMechanic.inFlingTime + Time.DeltaTime;
                    }
                    else if (inputController.leftTriggerPressed == true)
                    {
                        pv.Linear = forward * flingMechanic.force;
                        Debug.Log("pv ");
                        flingMechanic.inFling = true;
                        flingMechanic.inFlingTime = 0;
                    }

                }


            ).Run();


        }
    }


}