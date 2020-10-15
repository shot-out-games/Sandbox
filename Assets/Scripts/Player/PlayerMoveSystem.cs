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
            Vector3 stickInput = Vector3.zero;


            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (ref Translation translation, ref PhysicsVelocity pv,
                    ref ApplyImpulseComponent applyImpulseComponent,
                    in Pause pause, in DeadComponent dead,
                    in Entity entity, in InputControllerComponent inputController) =>
                {

                    if (pause.value == 0 && !dead.isDead)
                    {


                        damage = EntityManager.GetComponentData<ControlBarComponent>(entity).value;

                        if (inputController.buttonY_Pressed && damage < 25)
                        {
                            rewindPressed = true;
                        }



                    }

                }
            ).Run();



            Entities.WithoutBurst().ForEach(
                (
                    Entity e,
                    PlayerMove playerMove,
                    Animator animator,
                    ref PhysicsVelocity pv,
                    ref Translation translation,
                    in ApplyImpulseComponent applyImpulseComponent,
                    in InputControllerComponent inputController,
                    in RatingsComponent ratingsComponent
                ) =>
                {

                    float currentSpeed = ratingsComponent.gameSpeed;
                    Vector3 velocity = animator.deltaPosition / Time.DeltaTime * currentSpeed;

                    float leftStickX = inputController.leftStickX;
                    float leftStickY = inputController.leftStickY;

                    stickInput = new Vector3(leftStickX, 0, leftStickY);//x is controlled by rotation
                    stickSpeed = stickInput.sqrMagnitude;
                    //pv.Linear = applyImpulseComponent.Velocity;

                    animator.SetFloat("Vertical", stickSpeed);
                    //animator.SetFloat("Horizontal", stickInput.x);

                    velocity.y = 0;
                    pv.Linear = velocity;

                    animator.SetBool("Grounded", applyImpulseComponent.Grounded);


                    AudioSource audioSource = playerMove.audioSource;

                    if (math.abs(stickSpeed) >= .01f && math.abs(pv.Linear.y) <= .000001f)
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


                    translation.Value.y = 0;//change for jump use


                }
            ).Run();





        }

    }

    //[UpdateInGroup(typeof(TransformSystemGroup))]
    //[UpdateAfter(typeof(EndFrameLocalToParentSystem))]
    [UpdateAfter(typeof(PlayerRotateSystem))]


    public class PlayerRotateSystem : SystemBase
    {




        protected override void OnUpdate()
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


                 if (leftStickX != leftStickY && pause.value == 0 && !deadComponent.isDead)
                 {
                     Camera cam = playerMove.mainCam;
                     float slerpDampTime = playerMoveComponent.rotateSpeed;
                     Vector3 forward =
                          cam.transform.TransformDirection(Vector3.forward); //forward of camera to forward of world
                                                                             //local forward vector of camera will become world vector position that is passed to the forward vector of the player (target) rigidbody 
                                                                             //(The cam and player(target) vector will now always point in the same direction)


                     forward.y = 0f;

                     forward = forward.normalized;

                     Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

                     Vector3 targetDirection = (leftStickX * right + leftStickY * forward);

                     if (targetDirection.sqrMagnitude > 1f)
                     {
//                         targetDirection = targetDirection.normalized;
                     }

                     targetDirection = targetDirection.normalized;

                     quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());
                     //rotation.Value = targetRotation;

                     rotation.Value = math.slerp(rotation.Value, targetRotation, slerpDampTime);


                 }
             }
         ).Run();










        }

    }

















}
