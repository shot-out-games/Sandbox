using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;





namespace SandBox.Player
{


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]


    public class PlayerMoveSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            bool rewindPressed = false;
            float damage = 25;
            float stickSpeed = 0;
            Vector3 stickInput = Vector3.zero;




            Entities.WithoutBurst().WithNone<Pause>().ForEach(
                (
                    Entity e,
                    PlayerMove playerMove,
                    ref PhysicsVelocity pv,
                    ref Translation translation,
                    in ApplyImpulseComponent applyImpulseComponent,
                    in InputControllerComponent inputController,
                    in RatingsComponent ratingsComponent,
                    in PlayerMoveComponent playerMoveComponent
                ) =>
                {


                    if (HasComponent<PlayerJumpComponent>(e) == false)
                    {
                        //translation.Value.y = 0; //change for jump use
                    }


                    bool hasFling = HasComponent<FlingMechanicComponent>(e);
                    if (hasFling)
                    {
                        if (GetComponent<FlingMechanicComponent>(e).inFling == true)
                        {
                            return;
                        }
                    }



                    Camera cam = playerMove.mainCam;
                    Animator animator = playerMove.GetComponent<Animator>();



                    float currentSpeed = ratingsComponent.gameSpeed;
                    pv.Linear = float3.zero;
                    //Vector3 velocity = animator.deltaPosition / Time.DeltaTime * currentSpeed;

                    float leftStickX = inputController.leftStickX;
                    float leftStickY = inputController.leftStickY;

                    if (playerMoveComponent.move2d) leftStickY = 0;

                    stickInput = new Vector3(leftStickX, 0, leftStickY);//x is controlled by rotation
                    stickInput.Normalize();

                    stickSpeed = stickInput.sqrMagnitude;
                    //animator.SetFloat("Vertical", stickSpeed);
                    animator.SetFloat("Vertical", stickSpeed, playerMoveComponent.dampTime, Time.DeltaTime);
                    float3 fwd = cam.transform.forward;
                    float3 right = cam.transform.right;
                    //fwd = Vector3.forward;
                    //right = Vector3.right;
                    fwd = playerMove.transform.forward;
                    right = playerMove.transform.right;
                    fwd.y = 0;
                    right.y = 0;
                    fwd = math.normalize(fwd);
                    right = math.normalize(right);
                    
                    if (math.abs(stickSpeed) > .01f)
                    {
                        pv.Linear =  fwd * stickSpeed * currentSpeed;
                        pv.Linear.y = 0;
                    }

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


                    if(!applyImpulseComponent.BumpLeft && !applyImpulseComponent.BumpRight)
                        pv.Linear.y += applyImpulseComponent.NegativeForce;
                    else
                        pv.Linear.y += applyImpulseComponent.ApproachStairBoost;




                }
            ).Run();





        }

    }


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    //[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    [UpdateAfter(typeof(PlayerMoveSystem))]


    public class PlayerRotateSystem : SystemBase
    {

        //public float desiredRotationAngle;
        //public float CurrentRotationAngle;


        protected override void OnUpdate()
        {



            Entities.WithoutBurst().WithStructuralChanges().WithAll<PlayerComponent>().WithNone<Pause>().ForEach
         (
             (
                 PlayerMove playerMove,
                 ref Rotation rotation,
                 ref Translation translation,
                 ref PhysicsVelocity pv,
                 in DeadComponent deadComponent,
                 in PlayerMoveComponent playerMoveComponent,
                 in RatingsComponent ratingsComponent,
                 in InputControllerComponent inputController

             ) =>
             {
                 float leftStickX = inputController.leftStickX;
                 float leftStickY = inputController.leftStickY;

                 if (!deadComponent.isDead)
                 {
                     float slerpDampTime = playerMoveComponent.rotateSpeed;
                     var up = math.up();
                     bool haveInput = (math.abs(leftStickX) > float.Epsilon) || (math.abs(leftStickY) > float.Epsilon);

                     if (playerMoveComponent.snapRotation == true)
                     {

                         if (haveInput == true)
                         {
                             Vector3 forward = Vector3.forward;
                             forward.y = 0;
                             Vector3 right = Vector3.right;
                             Vector3 targetDirection = (leftStickX * right + leftStickY * forward);
                             targetDirection.Normalize();
                             quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());
                             rotation.Value = targetRotation;
                         }
                     }
                     else
                     {

                         if (haveInput == true)
                         {

                             //var forward = playerMove.mainCam.transform.TransformDirection(Vector3.forward);
                             //var right = playerMove.mainCam.transform.TransformDirection(Vector3.right);
                             var forward = playerMove.mainCam.transform.forward;
                             var right = playerMove.mainCam.transform.right;
                             //forward = Vector3.forward;
                             //right = Vector3.right;

                             forward.y = 0;

                             Vector3 targetDirection = (leftStickX * right + leftStickY * forward);
                             if (targetDirection.magnitude > .1)
                             {
                                 targetDirection.Normalize();
                                 quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());
                                 rotation.Value = math.slerp(rotation.Value, targetRotation, slerpDampTime * Time.DeltaTime);
                             }
                         }

                     }

                     if (playerMoveComponent.move2d)
                     {
                         translation.Value.z = playerMoveComponent.startPosition.z;
                     }




                 }
             }
         ).Run();










        }

    }

















}
