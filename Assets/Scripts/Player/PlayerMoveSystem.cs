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
                (ref PlayerMoveComponent playerMove, ref Translation translation, ref PhysicsVelocity pv,
                    ref ApplyImpulseComponent applyImpulseComponent,
                    in Pause pause, in DeadComponent dead,
                    in Entity entity, in InputController inputController) =>
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
                        leftStickX =math.abs(leftStickX) < .5 ? 0 : leftStickX;
                        leftStickY = math.abs(leftStickY) < .5 ? 0 : leftStickY;
                        applyImpulseComponent.stickX = leftStickX;
                        applyImpulseComponent.stickY = leftStickY;

                        leftStickY = 0;//2d but need Y for shooting still so save to applyimpulse above
                        Vector3 stickInput = new Vector3(leftStickX, 0, leftStickY);
                        stickSpeed = stickInput.sqrMagnitude;
                        pv.Linear = applyImpulseComponent.Velocity;
                        inputController.gameObject.GetComponent<Animator>().SetFloat("Speed", stickSpeed);
                        translation.Value.z = 0;


                    }

                }
            ).Run();



            Entities.WithoutBurst().ForEach(
                (
                    Entity e,
                    PlayerMove playerMove,
                    in PhysicsVelocity physicsVelocity
                ) =>
                {
                    AudioSource audioSource = playerMove.audioSource;

                    if (math.abs(stickSpeed) >= .01f  && math.abs(physicsVelocity.Linear.y) <= .000001f)
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
                        if(audioSource != null) audioSource.Stop();
                        if(playerMove.ps != null) playerMove.ps.Stop();

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

        }

    }

}
