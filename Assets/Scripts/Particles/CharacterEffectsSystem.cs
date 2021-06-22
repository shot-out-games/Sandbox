using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


public enum EffectType
{
    None = 0,
    Dead = 1,
    Damaged = 2,
    TwoClose = 3

}


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
                Debug.Log("shake");
                impulse.impulseSourceHitReceived.GenerateImpulse();

            }
        ).Run();





        //Entities.WithoutBurst().WithAll<AudioSourceComponent>().ForEach(
        //    (
        //        PowerItem powerItem, Entity e, AudioSource audioSource, PowerItemComponent powerItemComponent) =>
        //    {
        //        //impulse.impulseSourceHitReceived.GenerateImpulse();
        //        if (audioSource.isPlaying == false && powerItemComponent.enabled)
        //        {
        //            Debug.Log("play ");
        //            audioSource.Play();
        //        }

        //    }
        //).Run();



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
                ref EffectsComponent effectsComponent,
                in Animator animator,
                in EffectsManager effects) =>
            {


                AudioSource audioSource = effects.audioSource;

                if (effectsComponent.playEffectType == EffectType.TwoClose)
                {
                    Debug.Log("play effect two close");
                    //effectsComponent.playEffectType = EffectType.None;

                    if (effects.actorCloseEffectInstance)
                    {
                        if (effects.actorCloseEffectInstance.isPlaying == false && effectsComponent.playEffectAllowed)
                        {
                            effects.actorCloseEffectInstance.Play(true);
                        }
                        else if(effectsComponent.playEffectAllowed == false)
                        {
                            effects.actorCloseEffectInstance.Stop(true);

                        }
                    }
                    if (effects.actorCloseAudioClip)
                    {
                        audioSource.clip = effects.actorCloseAudioClip;
                        audioSource.Play();
                    }
                }

                if (deadComponent.playDeadEffects)//can probably just use playEffectType in effectsComponent TO DO
                {
                    deadComponent.playDeadEffects = false;

                    if (effects.actorDeadEffectInstance)
                    {
                        if (effects.actorDeadEffectInstance.isPlaying == false)
                        {
                            effects.actorDeadEffectInstance.Play(true);
                        }
                    }
                    if (effects.actorDeadAudioClip)
                    {
                        audioSource.clip = effects.actorDeadAudioClip;
                        audioSource.Play();
                    }
                }
                else
                {
                    bool hasDamage = HasComponent<DamageComponent>(e);
                    if (hasDamage == true)
                    {
                        var damageComponent = GetComponent<DamageComponent>(e);
                        if (damageComponent.DamageReceived <= .0001) return;
                        animator.SetInteger("HitReact", 1);
                        //Debug.Log("hit react");


                        if (effects.actorHurtEffectInstance)
                        {
                            effects.actorHurtEffectInstance.Play(true);
                        }
                        if (effects.actorHurtAudioClip)
                        {
                            audioSource.clip = effects.actorHurtAudioClip;
                            audioSource.Play();
                        }

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

            }
        ).Run();


    }




}

