using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[UpdateAfter(typeof(DeadSystem))]

public class CharacterEffectsSystem : SystemBase
{
    private float timer;



    protected override void OnUpdate()
    {
        timer += Time.DeltaTime;


        Entities.WithoutBurst().ForEach(
            (
                in DamageComponent damageComponent,
                in Impulse impulse) =>
            {
                if (damageComponent.DamageReceived == 0) return;
                //impulse.impulseSourceHitReceived.GenerateImpulse();

            }
        ).Run();



        Entities.WithoutBurst().ForEach(
            (
                in InputController input, in ControlBarComponent controlBar,
                in Impulse impulse) =>
            {
                //if (input.rightTriggerDown == true && controlBar.value < 25f) 
                //if (pause.value == 1) return;

                //if (input.leftTriggerDown == true || input.rightTriggerDown == true)
                //{
                //impulse.impulseSource.GenerateImpulse();
                //}

            }
        ).Run();









        Entities.WithoutBurst().WithNone<Pause>().ForEach(
            (
                Entity e,
                ref DeadComponent deadComponent,
                in EffectsComponent effectsComponent,
                in Animator animator,
                in EffectsManager effects) =>
            {

                //var damageGroup = GetComponentDataFromEntity<DamageComponent>(true);

                AudioSource audioSource = effects.audioSource;

                if (deadComponent.playDeadEffects)
                {
                    deadComponent.playDeadEffects = false;
                    Debug.Log("dead");
                    //deadComponent.justDead = false;

                    if (effects.actorDeadEffectInstance)
                    {
                        if (effects.actorDeadEffectInstance.isPlaying == false)
                        {
                            effects.actorDeadEffectInstance.Play(true);
                            Debug.Log("dead effect");
                        }
                    }
                    //if (audioSource.isPlaying == false)
                    //{
                        if (effects.actorDeadAudioClip)
                        {
                            audioSource.clip = effects.actorDeadAudioClip;
                            audioSource.Play();
                        }
                    //}
                }
                else
                {
                    bool hasDamage = HasComponent<DamageComponent>(e);
                    if (hasDamage == true)
                    {
                        var damageComponent = GetComponent<DamageComponent>(e);
                        if (damageComponent.DamageReceived <= .0001) return;
                        animator.SetInteger("HitReact", 1);
                        Debug.Log("hit react");


                        if (effects.actorHurtEffectInstance)
                        {
                            effects.actorHurtEffectInstance.Play(true);
                            //Debug.Log("hurt effect");
                        }
                        if (effects.actorHurtAudioClip)
                        {
                            //Debug.Log("sound");
                            audioSource.clip = effects.actorHurtAudioClip;
                            audioSource.Play();
                        }

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
                    if (effects.powerEnabledEffectInstance)
                    {
                        effects.powerEnabledEffectInstance.Stop(true);
                    }
                    return;
                }
                //
                if (power.enabled == true)
                {
                    if (effects.powerEnabledEffectInstance)
                    {
                        if (effects.powerTriggerEffectInstance.isPlaying == false)
                        {
                            effects.powerEnabledEffectInstance.Play(true);
                            Debug.Log("power enabled");
                        }
                    }
                    if (effects.powerEnabledAudioClip)
                    {
                        audioSource.PlayOneShot(effects.powerEnabledAudioClip);
                    }
                }
                else if (power.triggered == true)
                {
                    if (effects.powerTriggerEffectInstance)
                    {
                        if (effects.powerTriggerEffectInstance.isPlaying == false)
                        {
                            effects.powerTriggerEffectInstance.Play(true);
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

