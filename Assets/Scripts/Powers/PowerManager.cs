using System.Collections;
using System.Collections.Generic;
using SandBox.Player;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;




public enum PowerType
{
    None = 0,
    Speed = 1,
    Health = 2,
    Control = 3,
}

public struct Speed : IComponentData
{
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
    public bool enabled;
    public float healthMultiplier;
}

public struct ControlPower : IComponentData
{
    public bool enabled;
    public float controlMultiplier;
}

public class PowerManager : MonoBehaviour, IConvertGameObjectToEntity
{
    //[Header("Speed")]
    //[SerializeField] private float speedTimeOn = 3.0f;
    //[SerializeField] private float speedMultiplier = 3.0f;

    //[Header("Health")]
    //[SerializeField] private float healthMultiplier = 1.5f;



    private EntityManager manager;
    private Entity e;

    private void OnTriggerEnter(Collider other)
    {
        PowerTrigger(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PowerTrigger(collision.gameObject);
    }


    void PowerTriggerDots(int powerType)
    {
        var powerItem = manager.GetComponentData<PowerItemComponent>(e);
        //int powerType = powerItem.powerType;


        if (powerType == (int)PowerType.Control)
        {
            var control = manager.GetComponentData<ControlPower>(e);
            control.enabled = true;
            control.controlMultiplier = powerItem.healthMultiplier;
            manager.SetComponentData<ControlPower>(e, control);
        }
        if (powerType == (int)PowerType.Health)
        {
            var power = manager.GetComponentData<HealthPower>(e);
            power.enabled = true;
            power.healthMultiplier = powerItem.healthMultiplier;
            manager.SetComponentData<HealthPower>(e, power);
        }


    }

    private void PowerTrigger(GameObject go)
    {
        Debug.Log("Power");
        Entity other_e = Entity.Null;
        if (go.GetComponent<PowerItem>())///if has this mono then has component poweritemcomponent - check??
        {
            other_e = go.GetComponent<PowerItem>().e;
        }
        else
        {
            return;
        }

        if (manager.HasComponent<PowerItemComponent>(other_e) == false) return;

        bool active = manager.GetComponentData<PowerItemComponent>(other_e).active;
        if (active == false)
        {
            return;
        }

        var powerItem = manager.GetComponentData<PowerItemComponent>(other_e);
        int powerType = powerItem.powerType;

        if (powerType == (int)PowerType.Speed)
        {
            var speed = manager.GetComponentData<Speed>(e);
            speed.enabled = true;
            speed.timeOn = powerItem.speedTimeOn;
            speed.multiplier = powerItem.speedTimeMultiplier;
            manager.SetComponentData<Speed>(e, speed);
        }

        if (powerType == (int)PowerType.Health)
        {
            var health = manager.GetComponentData<HealthPower>(e);
            health.enabled = true;
            health.healthMultiplier = powerItem.healthMultiplier;
            manager.SetComponentData<HealthPower>(e, health);
        }



        //set other item to inactive
        var item = manager.GetComponentData<PowerItemComponent>(other_e);
        item.active = false;
        manager.SetComponentData<PowerItemComponent>(other_e, item);

        go.SetActive(false);
        //destroy optional  as makes item.active irrelevant but now no need to cleanup the item elsewhere but option still
        //Destroy(go);
        //manager.DestroyEntity(other_e);

    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        manager = dstManager;
        e = entity;
        dstManager.AddComponentData(entity, new Speed
        {
            enabled = false,
            timer = 0,
            timeOn = 0,
            startTimer = false,
            originalSpeed = 0,
            multiplier = 0,
        }
        );

        dstManager.AddComponentData(entity, new HealthPower
            {
                enabled = false,
                healthMultiplier = 0
            }
        );

        dstManager.AddComponentData(entity, new ControlPower
            {
                enabled = false,
                controlMultiplier = 0
            }
        );


    }
}


//[UpdateAfter(typeof(PlayerMoveSystem))]

public class PowersSystem : JobComponentSystem
{

    protected override void OnCreate()
    {

    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        Entities.WithoutBurst().ForEach(
            (
                    ref Speed speed, ref RatingsComponent ratings

                ) =>
            {

                if (speed.startTimer == false && speed.enabled == true)
                {
                    speed.triggered = true;
                    speed.startTimer = true;
                    speed.timer = 0;
                    speed.originalSpeed = ratings.speed;
                    ratings.speed = ratings.speed * speed.multiplier;
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
                    ratings.speed = speed.originalSpeed;
                    speed.originalSpeed = 0;
                }

            }
        ).Run();

        Entities.WithoutBurst().ForEach(
            (
                ref HealthPower healthPower, ref HealthComponent healthComponent, in RatingsComponent ratings

            ) =>
            {
                if (healthPower.enabled == true)
                {
                    healthPower.enabled = false;
                    healthComponent.TotalDamageReceived = healthComponent.TotalDamageReceived * healthPower.healthMultiplier;
                    //Rare used if multiplier is > 1 meaning health damage increased
                    if (healthComponent.TotalDamageReceived > ratings.maxHealth)
                    {
                        healthComponent.TotalDamageReceived = ratings.maxHealth;
                    }
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


        return default;
    }




}

