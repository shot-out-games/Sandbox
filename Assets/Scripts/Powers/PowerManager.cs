using System.Collections;
using System.Collections.Generic;
using SandBox.Player;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;




public enum PowerType
{
    None = 0,
    Speed = 1,
    Health = 2,
    Control = 3,
}


public struct TestComponent : IComponentData
{
    public float value;
}

public struct ParticleSystemComponent : IComponentData
{
    public float value;
    public bool followActor;
    public Entity pickedUpActor;
}


public struct Speed : IComponentData
{

    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool triggered;
    public bool enabled;
    public bool startTimer;
    public float timer;
    public float timeOn;
    public float originalSpeed;
    public float multiplier;
}

public struct HealthPower : IComponentData
{
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public float healthMultiplier;
}

public struct ControlPower : IComponentData
{
    public Entity psAttached;
    public Entity pickedUpActor;
    public Entity itemEntity;
    public bool enabled;
    public float controlMultiplier;
}

public class PowerManager : MonoBehaviour, IConvertGameObjectToEntity
{
    



    private EntityManager manager;
    private Entity e;




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        manager = dstManager;
        e = entity;



    }
}


//[UpdateAfter(typeof(PlayerMoveSystem))]

public class PowersSystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }



    protected override void OnUpdate()
    {

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();


        //test
        Entities.WithoutBurst().WithNone<TriggerComponent>().ForEach((ParticleSystem ps, Entity e) =>
            {

                //Debug.Log("ps " + e);
                //ecb.AddComponent<TriggerComponent>(e);

            }
        ).Run();

        Entities.WithoutBurst().ForEach((ParticleSystemComponent ps, Entity e) =>
            {

                float3 value = GetComponent<Translation>(ps.pickedUpActor).Value;


                if (ps.followActor == false)
                {
                    ecb.DestroyEntity(e);
                    return;
                }


                SetComponent(e, new Translation { Value = value });


            }
        ).Run();





        Entities.WithoutBurst().ForEach(
            (
                    ref Speed speed, ref RatingsComponent ratings,
                        in Entity e


                ) =>
            {

                if (speed.startTimer == false && speed.enabled == true)
                {
                    ecb.DestroyEntity(speed.itemEntity);
                    speed.triggered = true;
                    speed.startTimer = true;
                    speed.timer = 0;
                    //speed.originalSpeed = ratings.gameSpeed;
                    ratings.gameSpeed = ratings.gameSpeed * speed.multiplier;
                }
                else if (speed.enabled && speed.timer < speed.timeOn)
                {
                    speed.triggered = false;
                    speed.timer += Time.DeltaTime;
                }
                else if (speed.enabled)
                {
                    speed.startTimer = false;
                    speed.timer = 0;
                    speed.enabled = false;
                    ratings.gameSpeed = ratings.speed;
                    Debug.Log("ps att " + speed.psAttached);
                    ecb.DestroyEntity(speed.psAttached);
                    //ecb.DestroyEntity(speed.);
                    //speed.originalSpeed = 0;
                    //ecb.RemoveComponent<Speed>(e);

                }

            }
        ).Run();

        Entities.WithoutBurst().ForEach(
            (
                ref HealthPower healthPower, ref HealthComponent healthComponent, in RatingsComponent ratings, in Entity e

            ) =>
            {
                if (healthPower.enabled == true)
                {
                    Debug.Log("hp");
                    healthPower.enabled = false;
                    healthComponent.TotalDamageReceived = healthComponent.TotalDamageReceived * healthPower.healthMultiplier;
                    //Rare used if multiplier is > 1 meaning health damage increased
                    if (healthComponent.TotalDamageReceived > ratings.maxHealth)
                    {
                        healthComponent.TotalDamageReceived = ratings.maxHealth;
                    }
                    ecb.RemoveComponent<HealthPower>(e);
                    ecb.DestroyEntity(healthPower.itemEntity);
                    ecb.DestroyEntity(healthPower.psAttached);


                }

            }
        ).Run();

        Entities.WithoutBurst().ForEach(
            (
                ref ControlPower healthPower, ref ControlBarComponent healthComponent

            ) =>
            {
                if (healthPower.enabled == true)
                {
                    healthPower.enabled = false;
                    healthComponent.value = healthComponent.value * healthPower.controlMultiplier;
                    //Rare used if multiplier is > 1 meaning health damage increased
                    if (healthComponent.value > healthComponent.maxHealth)
                    {
                        healthComponent.value = healthComponent.maxHealth;
                    }
                }

            }
        ).Run();

        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);



    }




}

