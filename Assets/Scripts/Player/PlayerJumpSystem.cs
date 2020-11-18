using System;
using SandBox.Player;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using ForceMode = UnityEngine.ForceMode;





namespace SandBox.Player
{


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]

    public class PlayerJumpSystem2D : SystemBase
    {
        int frames = 0;
        private int airFrames = 0;

        float buttonHeldFrames = 0;
        //private int fallingFramesCounter = 0;


        protected override void OnUpdate()
        {

            Entities.WithoutBurst().WithNone<Pause>().ForEach(
                (
                    (
                        PlayerJump2D playerJump,
                        ref Translation translation,
                        ref PhysicsVelocity pv,
                        ref ApplyImpulseComponent applyImpulseComponent,
                        ref PlayerJumpComponent playerJumpComponent,
                        in InputControllerComponent inputController,
                        in Entity e

                    ) =>
                    {



                        bool variableJump = true;

                        float leftStickX = inputController.leftStickX;
                        float leftStickY = inputController.leftStickY;

                        bool button_a = inputController.buttonA_Pressed;
                        bool button_a_held = inputController.buttonA_held;

                        float airForceAdd = 0;
                        float3 velocity = pv.Linear;

                        float gameToDefaultJumpForce = playerJumpComponent.gameStartJumpGravityForce /
                                                       playerJumpComponent.startJumpGravityForce;

                        float jumpFrames =
                            gameToDefaultJumpForce *
                            playerJumpComponent.jumpFramesToPeak; //increases if now jumping higher
                        float originalJumpFrames = playerJumpComponent.jumpFramesToPeak;
                        float jumpPower = playerJumpComponent.gameStartJumpGravityForce;
                        float originalJumpPower = playerJumpComponent.startJumpGravityForce;

                        float standardJumpHeight =
                            originalJumpPower * originalJumpFrames; //total height of jump at peak - ref only
                        float hiJumpMultiplier = 2f; //multiply jump power by this - frames 
                        float hiJumpAirFramesMax = jumpFrames * hiJumpMultiplier;

                        if (applyImpulseComponent.InJump == false)
                        {
                            frames = 0;
                            airFrames = 0;
                            playerJumpComponent.JumpStage = JumpStages.Ground;
                            applyImpulseComponent.hiJump = false;


                        }

                        if (applyImpulseComponent.Falling)
                        {
                            pv.Linear.y += applyImpulseComponent.NegativeForce;
                            return;
                        }

                        //pv.Linear.z = leftStickY;

                        if ((button_a == true || button_a_held == true) && applyImpulseComponent.InJump == false &&
                            frames == 0)
                        {
                            applyImpulseComponent.InJump = true;
                            applyImpulseComponent.Grounded = false;
                            applyImpulseComponent.Falling = false;
                            applyImpulseComponent.hiJump = false;

                            frames = 1;
                            //Debug.Log(" start fr " + frames);
                            playerJump.GetComponent<Animator>().SetTrigger("JumpStage");
                            playerJump.GetComponent<Animator>().applyRootMotion = false;
                            playerJumpComponent.JumpStage = JumpStages.JumpStart;
                            velocity = new float3(pv.Linear.x, originalJumpPower, pv.Linear.z);
                            //pv.Linear = vel;
                        }
                        else if (frames >= 1 && frames <= originalJumpFrames && applyImpulseComponent.InJump == true &&
                                 applyImpulseComponent.Grounded == false && applyImpulseComponent.Falling == false)
                        {
                            frames = frames + 1;
                            if (frames == originalJumpFrames - 2 && button_a_held == true && variableJump == false
                            ) //make sure number here less than jump up frames at some point
                            {
                                buttonHeldFrames = 1;
                            }
                            else if (frames == originalJumpFrames - 1 && button_a_held == true && variableJump == false
                            ) //make sure number here less than jump up frames at some point
                            {
                                buttonHeldFrames = 2;
                            }
                            else if (frames == originalJumpFrames && button_a_held == true &&
                                     (buttonHeldFrames == 2 || variableJump == true)
                            ) //make sure number here less than jump up frames at some point
                            {
                                applyImpulseComponent.hiJump = true;
                                buttonHeldFrames = 0;
                            }
                            else
                            {
                                buttonHeldFrames = 0;
                            }

                            velocity = new float3(pv.Linear.x, originalJumpPower, leftStickY);
                        }
                        else if (applyImpulseComponent.hiJump == true &&
                                 (frames > 1 && airFrames < hiJumpAirFramesMax && variableJump == false)
                                 ||
                                 (frames > 1 && airFrames < hiJumpAirFramesMax && variableJump == true &&
                                  button_a_held == true &&
                                  button_a == false)
                        ) //6 on higher jump after 6th frame held - very static could problem like if game has quick jump with low jump frames it will go up after jumpframes peak
                        {

                            frames++;
                            airFrames++;
                            velocity.y = jumpPower * hiJumpMultiplier;

                        }
                        else if (applyImpulseComponent.hiJump == true &&
                                 (airFrames >= hiJumpAirFramesMax
                                  || variableJump == true && (button_a_held == false || button_a == false)))
                            //6 on higher jump after 6th frame held - very static could problem like if game has quick jump with low jump frames it will go up after jumpframes peak
                        {
                            applyImpulseComponent.hiJump = false;
                        }
                        else if (playerJumpComponent.JumpStage == JumpStages.JumpStart)
                        {

                            frames++;
                            playerJumpComponent.JumpStage = JumpStages.JumpUp;
                            airForceAdd = leftStickX * playerJumpComponent.airForce;
                            velocity = new float3(pv.Linear.x, pv.Linear.y, pv.Linear.z);
                            velocity.x += airForceAdd;
                        }
                        else if (playerJumpComponent.JumpStage == JumpStages.JumpUp)
                        {
                            frames++;
                            airForceAdd = leftStickX * playerJumpComponent.airForce;
                            velocity = new float3(pv.Linear.x, pv.Linear.y, pv.Linear.z);
                            velocity.x += airForceAdd;
                        }


                        pv.Linear = new float3(velocity.x, velocity.y, velocity.z);
                        if (playerJumpComponent.JumpStage != JumpStages.Ground)
                        {
                            pv.Linear.y += applyImpulseComponent.NegativeForce;
                        }





                        if (button_a == true && frames == 1)
                        {
                            AudioSource audioSource = playerJump.audioSource;
                            if (playerJump.audioClip && audioSource)
                            {
                                audioSource.PlayOneShot(playerJump.audioClip);
                            }

                            if (playerJump.ps)
                            {
                                playerJump.ps.transform.SetParent(playerJump.transform);
                                playerJump.ps.Play(true);
                            }
                        }

                    }
                )
            ).Run();

        }


    }
}

