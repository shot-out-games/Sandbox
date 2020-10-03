using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;





namespace SandBox.Player
{


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]



    public class PlayerMoveSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            bool rewindPressed = false;
            float damage = 25;
            float stickSpeed = 0;

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (ref Translation translation, ref PhysicsVelocity pv,
                    ref ApplyImpulseComponent applyImpulseComponent,
                    in Pause pause, in DeadComponent dead,
                    in Entity entity, in InputControllerComponent inputController) =>
                {

                    if (pause.value == 0 && !dead.isDead)
                    {

                        //if (!EntityManager.HasComponent<PlayerTurboComponent>(entity))
                        // playerMove.currentSpeed = ratingsComponent.speed;

                        damage = EntityManager.GetComponentData<ControlBarComponent>(entity).value;

                        if (inputController.buttonY_Pressed && damage < 25)
                        {
                            rewindPressed = true;
                        }


                        float leftStickX = inputController.leftStickX;
                        float leftStickY = inputController.leftStickY;
                        leftStickX = math.abs(leftStickX) < .5 ? 0 : leftStickX;
                        leftStickY = math.abs(leftStickY) < .5 ? 0 : leftStickY;
                        applyImpulseComponent.stickX = leftStickX;
                        applyImpulseComponent.stickY = leftStickY;

                        //leftStickY = 0;//2d but need Y for shooting still so save to applyimpulse above


                        Vector3 stickInput = new Vector3(leftStickX, 0, leftStickY);
                        stickSpeed = stickInput.sqrMagnitude;
                        pv.Linear = applyImpulseComponent.Velocity;
                        //translation.Value.z = 0;


                    }

                }
            ).Run();



            Entities.WithoutBurst().ForEach(
                (
                    Entity e,
                    PlayerMove playerMove,
                    Animator animator,
                    in PhysicsVelocity physicsVelocity
                ) =>
                {
                    animator.SetFloat("Speed", stickSpeed);


                    AudioSource audioSource = playerMove.audioSource;

                    if (math.abs(stickSpeed) >= .01f && math.abs(physicsVelocity.Linear.y) <= .000001f)
                    {
                        if (playerMove.clip && audioSource)
                        {
                            audioSource.pitch = stickSpeed * 2;
                            if (audioSource.isPlaying == false)
                            {
                                audioSource.clip = playerMove.clip;
                                audioSource.Play();
                                //Debug.Log("clip " + audioSource.clip);

                            }

                        }

                        if (playerMove.ps)
                        {
                            if (playerMove.ps.isPlaying == false)
                            {
                                playerMove.ps.transform.SetParent(playerMove.transform);
                                playerMove.ps.Play(true);
                            }
                        }
                    }
                    else
                    {
                        if (audioSource != null) audioSource.Stop();
                        if (playerMove.ps != null) playerMove.ps.Stop();

                    }




                }
            ).Run();

            Entities.WithoutBurst().ForEach(
                (
                    in Entity e,
                    in Impulse impulse) =>
                {
                    if (rewindPressed && damage < 25)
                    {
                        impulse.impulseSource.GenerateImpulse();
                    }

                }
            ).Run();



            Entities.WithoutBurst().ForEach(
                (Entity e, ref RewindComponent rewindComponent) =>
                {

                    if (rewindPressed)
                    {
                        rewindComponent.@on = !rewindComponent.@on;
                        rewindComponent.pressed = true;
                        Debug.Log("rew " + rewindComponent.@on);
                    }
                    else
                    {
                        rewindComponent.pressed = false;
                    }


                }
            ).Run();



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
                     //float slerpDampTime = playerMoveComponent.rotateSlerpDampTime;
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


                     quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());

                     //rotation.Value = targetRotation;

                     Vector3 desiredDirection = Vector3.Normalize(new Vector3(leftStickX, 0f, leftStickY));
                     if (desiredDirection != Vector3.zero)
                     {
                         //quaternion _rotation = quaternion.LookRotation(desiredDirection, math.up());
                         quaternion newRotation = math.slerp(playerMove.transform.rotation, targetRotation, playerMoveComponent.rotateSpeed * Time.DeltaTime);
                         rotation.Value = newRotation;
                     }



                 }
             }
         ).Run();










        }

    }

}
