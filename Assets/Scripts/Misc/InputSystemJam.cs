using System.Diagnostics;
using Rewired;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;


public struct InputRestrictComponent : IComponentData
{
    public bool lt;
    public bool v;
    public bool h;
}

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(InputControllerSystemUpdate))]

public class InputSystemJam : SystemBase
{
    protected override void OnUpdate()
    {

        bool controlFireUsed = false;
        bool controlVerticalUsed = false;
        bool controlHorizontalUsed = false;
        bool rewindUsed = false;

        Entities.WithoutBurst().ForEach((EnemyComponent enemyComponent, ref RewindComponent rewind, ref GunComponent gunComponent,
            ref Translation translation, in Rotation rotation) =>
        {

            if (!ReInput.isReady) return;
            Player player = ReInput.players.GetPlayer(0);
            bool lt = player.GetButton("LeftTrigger");
            bool h = player.GetNegativeButton("Move Horizontal") || player.GetButton("Move Horizontal");
            bool v = player.GetNegativeButton("Move Vertical") || player.GetButton("Move Vertical");
            bool rightTriggerDown = player.GetButtonDown("RightTrigger");
            if (rightTriggerDown == true)
            {
                controlFireUsed = true;
            }


            if (rewind.on)
            {
                rewindUsed = true;
            }
            //restrictComponent.lt = false;
            //restrictComponent.h = false;
            //restrictComponent.v = false;

            //if (lt == true)
            ////if (lt == true && gunComponent.Disable == true)
            //{
            //    restrictComponent.lt = true;
            //    controlFireUsed = true;
            //}

            //if (h == true)
            //{
            //    restrictComponent.h = true;
            //    controlHorizontalUsed = true;
            //}
            //if (v == true)
            //{
            //    restrictComponent.v = true;
            //    controlVerticalUsed = true;
            //}

        }).Run();


        Entities.WithoutBurst().ForEach((PlayerComponent playerComponent, ref ControlBarComponent controlBarComponent) =>
        {
            //if (controlBarComponent.value > 25) return;

            //if (rewindUsed)
            //{
            //    controlBarComponent.value = controlBarComponent.value + 1f * Time.DeltaTime;
            //}
            //if (controlFireUsed)
            //{
            //    controlBarComponent.value = controlBarComponent.value + 1f;
            //}

            //if (controlHorizontalUsed == true)
            //{
            //    controlBarComponent.value = controlBarComponent.value + .2f * Time.DeltaTime;
            //}
            //if (controlVerticalUsed == true)
            //{
            //    controlBarComponent.value = controlBarComponent.value + .2f * Time.DeltaTime;
            //}

            //Debug.Log("cbc " + controlBarComponent.value);

        }).Run();


    }
}
