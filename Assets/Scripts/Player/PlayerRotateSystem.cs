using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

namespace SandBox.Player
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

    public class PlayerRotateSystem : JobComponentSystem
    {





        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities.WithoutBurst().ForEach
            (
                (
                    PlayerMove playerMove,
                    ref Rotation rotation,
                    in Pause pause, in DeadComponent deadComponent,
                    in PlayerMoveComponent playerMoveComponent,
                    in InputController inputController

                ) =>
                {
                    float leftStickX = inputController.leftStickX;
                    float leftStickY = inputController.leftStickY;
                    bool rotating = inputController.rotating;


                    if (rotating && leftStickX != leftStickY && pause.value == 0 && !deadComponent.isDead)
                    {
                        Camera cam = playerMove.mainCam;
                        float slerpDampTime = playerMoveComponent.rotateSlerpDampTime;
                        Quaternion camRotation = cam.transform.rotation;
                        camRotation.x = 0;
                        camRotation.z = 0;
                        // we need some axis derived from camera but aligned with floor plane
                        Vector3 forward =
                                cam.transform.TransformDirection(Vector3.forward); //forward of camera to forward of world
                                                                                   //local forward vector of camera will become world vector position that is passed to the forward vector of the player (target) rigidbody 
                                                                                   //(The cam and player(target) vector will now always point in the same direction)

                        forward.y = 0f;

                        forward = forward.normalized;
                        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
                        Vector3 targetDirection = (leftStickX * right + leftStickY * forward);
                        if (targetDirection.sqrMagnitude > 1f)
                        {
                            targetDirection = targetDirection.normalized;
                        }



                        //float3 direction = playerPos - trans.Value;
                        //direction.y = 0f;

                        // 6
                        //rot.Value = quaternion.LookRotation(direction, math.up());

                        quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());
                        //rotation.Value = math.slerp(rotation.Value, targetRotation, Time.DeltaTime * 15f);
                        rotation.Value = targetRotation;

                    }
                }
            ).Run();

            return default;


        }
    }
}
