using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


[System.Serializable]
public struct PowerItemComponent : IComponentData
{
    public bool active;
    public int powerType;
    public float speedTimeOn;
    public float speedTimeMultiplier;
    public float healthMultiplier;

}





public class PowerItem : MonoBehaviour, IConvertGameObjectToEntity
{
    public PowerType powerType;


    [SerializeField]
    bool active = true;
    public Entity e;
    EntityManager manager;

    [Header("Speed")]
    [SerializeField] private float speedTimeOn = 3.0f;
    [SerializeField] private float speedMultiplier = 3.0f;

    [Header("Health")]
    [SerializeField] private float healthMultiplier = .75f;



    void Update()
    {
        if (manager == default || e == Entity.Null) return;
        if (manager.HasComponent<PowerItemComponent>(e) == false) return;

        PowerItemComponent powerItem = manager.GetComponentData<PowerItemComponent>(e);
        if (powerItem.active == false)
        {
            gameObject.SetActive(false);
        };

    }





    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        manager.AddComponentData<PowerItemComponent>(entity, new PowerItemComponent
        {
            active = active,
            powerType = (int)powerType,
            speedTimeOn = speedTimeOn,
            speedTimeMultiplier =  speedMultiplier,
            healthMultiplier = healthMultiplier
        });

    }


}
