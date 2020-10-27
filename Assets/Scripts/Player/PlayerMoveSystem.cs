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
    [UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]


    public class PlayerMoveSystem : SystemBase
    {


        protected override void OnUpdate()
        {
            bool rewindPressed = false;
            float damage = 25;
            float stickSpeed = 0;
            Vector3 stickInput = Vector3.zero;




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

                    translation.Value.y = 0;//change for jump use


                    bool hasFling = HasComponent<FlingMechanicComponent>(e);
                    if(hasFling)
                    {
                       if(EntityManager.GetComponentData<FlingMechanicComponent>(e).inFling == true)
                        {
                            return;
                        }
                    }



                    Camera cam = playerMove.mainCam;



                    float currentSpeed = ratingsComponent.gameSpeed;
                    //Vector3 velocity = animator.deltaPosition / Time.DeltaTime * currentSpeed;

                    float leftStickX = inputController.leftStickX;
                    float leftStickY = inputController.leftStickY;

                    //Vector3 moveDir = cam.transform.forward * leftStickY + cam.transform.right * leftStickX;
                    //moveDir.Normalize();

                    stickInput = new Vector3(leftStickX, 0, leftStickY);//x is controlled by rotation


                    stickSpeed = stickInput.sqrMagnitude;
                    animator.SetFloat("Vertical", stickSpeed, .03f, Time.DeltaTime);
                    float3 fwd = cam.transform.forward;
                    float3 right = cam.transform.right;
                    fwd.y = 0;
                    right.y = 0;
                    fwd = math.normalize(fwd);
                    right = math.normalize(right);

                    if (math.abs(stickSpeed) > .01f)
                    {
                        pv.Linear = right * leftStickX * currentSpeed + fwd * leftStickY * currentSpeed;
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




                }
            ).Run();





        }

    }


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
    [UpdateAfter(typeof(PlayerMoveSystem))]


    public class PlayerRotateSystem : SystemBase
    {

        //public float desiredRotationAngle;
        public float CurrentRotationAngle;



        protected override void OnUpdate()
        {



            Entities.WithoutBurst().ForEach
         (
             (
                 PlayerMove playerMove,
                 ref Rotation rotation,
                 ref PhysicsVelocity pv,
                 in Pause pause, in DeadComponent deadComponent,
                 in PlayerMoveComponent playerMoveComponent,
                 in RatingsComponent ratingsComponent,
                     in InputController inputController

             ) =>
             {
                 float leftStickX = inputController.leftStickX;
                 float leftStickY = inputController.leftStickY;

                 if (pause.value == 0 && !deadComponent.isDead)
                 {
                     float slerpDampTime = playerMoveComponent.rotateSpeed;
                     var up = math.up();
                    

                     bool haveInput = (math.abs(leftStickX) > float.Epsilon) || (math.abs(leftStickY) > float.Epsilon);


                 if (haveInput)
                     {

                         Vector3 forward = playerMove.mainCam.transform.forward;
                         forward = playerMove.transform.forward;
                         forward = Vector3.forward;
                         forward.y = 0;
                         Vector3 right = playerMove.mainCam.transform.right;
                         right = playerMove.transform.right;
                         right = Vector3.right;
                         //Vector3 right = Quaternion.Euler(0, 90, 0) * forward;

                         Vector3 targetDirection = (leftStickX * right + leftStickY * forward);
                         targetDirection.Normalize();


                         quaternion targetRotation = quaternion.LookRotation(targetDirection, math.up());

                         //rotation.Value = math.slerp(rotation.Value, targetRotation, slerpDampTime * Time.DeltaTime);

                         rotation.Value = targetRotation;

                         //rotation.Value = math.slerp(rotation.Value, targetRotation, slerpDampTime);

                         //desiredRotationAngle = Vector3.Angle(playerMove.transform.forward, targetRotation);
                         //var crossProduct = Vector3.Cross(playerMove.transform.forward, forward).y;
                         //if (crossProduct < 0)
                         //{
                         //    desiredRotationAngle *= -1;
                         //}


                         //if (desiredRotationAngle > 10 || desiredRotationAngle < -10)
                         //{
                         //    rotation.Value = math.slerp(rotation.Value, targetRotation, slerpDampTime);
                         //   // transform.Rotate(Vector3.up * desiredRotationAngle * rotationSpeed * Time.deltaTime);
                         //}
                     }

                 }
             }
         ).Run();










        }

    }

















}
