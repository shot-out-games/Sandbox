using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class CharacterEffectsSystem : SystemBase
{
    private float timer;



    protected override void OnUpdate()
    {
        timer += Time.DeltaTime;


        Entities.WithoutBurst().ForEach(
            (
                in Pause pause,
                in DamageComponent damageComponent,
                in Impulse impulse) =>
            {
                if (damageComponent.DamageReceived == 0 || pause.value == 1) return;
                impulse.impulseSource.GenerateImpulse();

            }
        ).Run();



        Entities.WithoutBurst().ForEach(
            (
                InputController input, ControlBarComponent controlBar, in Pause pause,
                in Impulse impulse) =>
            {
                //if (input.rightTriggerDown == true && controlBar.value < 25f) 
                if (pause.value == 1) return;

                //if (input.leftTriggerDown == true || input.rightTriggerDown == true)
                //{
                    //impulse.impulseSource.GenerateImpulse();
                //}

            }
        ).Run();






        Entities.WithoutBurst().ForEach(
            (
                Entity e,
                in Pause pause,
                in EffectsComponent effectsComponent,
                in DamageComponent damageComponent,
                in Transform transform,
                in Animator animator,
                in EffectsManager effects) =>
            {
                if (damageComponent.DamageReceived == 0 || pause.value == 1) return;

                bool skip = false;
                //if (EntityManager.HasComponent(e, typeof(EnemyComponent)))
                //{
                //    skip = EntityManager.GetComponentData<EnemyComponent>(e).invincible;
                //}

                if (skip == false)
                {

                    animator.SetInteger("HitReact", 1);

                    AudioSource audioSource = effects.audioSource;


                    if (effects.playerHurtEffect)
                    {
                        timer = 0f;
                        effects.playerHurtEffect.Play(true);
                    }

                    if (effects.playerHurtAudioClip)
                    {
                        //audioSource.PlayOneShot(effects.playerHurtAudioClip);
                        audioSource.clip = effects.playerHurtAudioClip;
                        audioSource.Play();
                    }
                }
            }
        ).Run();


        Entities.WithoutBurst().ForEach(
            (
                in Speed power,
                 in AudioSource audioSource, in EffectsManager effects) =>
            {
                if (power.enabled == false)
                {
                    if (effects.powerEnabledEffect)
                    {
                        effects.powerEnabledEffect.Stop(true);
                    }
                    return;
                }

                if (power.enabled == true)
                {
                    if (effects.powerEnabledEffect)
                    {
                        if (effects.powerTriggerEffect.isPlaying == false)
                        {
                            effects.powerEnabledEffect.Play(true);
                            Debug.Log("power enabled");
                        }
                    }
                    if (effects.powerEnabledAudioClip) audioSource.PlayOneShot(effects.powerEnabledAudioClip);
                }
                else if (power.triggered == true)
                {
                    if (effects.powerTriggerEffect)
                    {
                        if (effects.powerTriggerEffect.isPlaying == false)
                        {
                            effects.powerTriggerEffect.Play(true);
                            Debug.Log("power triggered");
                        }
                    }

                    if (effects.powerTriggerAudioClip)
                    {
                        audioSource.PlayOneShot(effects.powerTriggerAudioClip);
                    }
                }


            }
        ).Run();


        Entities.WithoutBurst().ForEach
        (
            (ref EffectsComponent effectsComponent,
                in LevelCompleteComponent goal, in Entity entity, in EffectsManager effects, in AudioSource audioSource) =>
            {
                if (goal.active == true || effectsComponent.soundPlaying == true) return;
                if (effects.playerLevelCompleteClip)
                {
                    audioSource.PlayOneShot(effects.playerLevelCompleteClip);
                    effectsComponent.soundPlaying = true;
                }

            }
        ).Run();


    }




}

