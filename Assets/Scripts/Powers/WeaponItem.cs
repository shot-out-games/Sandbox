﻿using RootMotion.FinalIK;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


//[System.Serializable]
//public struct WeaponItemComponent : IComponentData
//{
//    public Entity e;
//    public bool active;
//    public int weaponType;
//    public bool pickedUp;
//}

[System.Serializable]
public struct WeaponItemComponent : IBufferElementData
{
    public Entity e;
    public bool active;
    public int weaponType;
    public bool pickedUp;
    public bool special;//for ld
    public bool reset;
}




public class WeaponItem : MonoBehaviour, IConvertGameObjectToEntity
{

    public Weapons weaponData;

    //public WeaponType weaponType; //in weapondata
    private AudioSource audioSource;
    [HideInInspector] public InteractionObject interactionObject;//set from wea

    [SerializeField]
    bool active = true;
    public Entity e;
    EntityManager manager;
    public bool special;
    public bool reset = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactionObject = GetComponent<InteractionObject>();
    }


    void Update()
    {
        if (manager == default || e == Entity.Null) return;
        if (manager.HasComponent<WeaponItemComponent>(e) == false) return;



        //var item = manager.GetBuffer<WeaponItemComponent>(e);
        float3 pos = new Vector3 { x = transform.position.x, y = .09f, z = transform.position.z };

        if (reset == true)
        {
            var tr = manager.GetComponentData<Translation>(e);
            tr.Value = pos;
            manager.SetComponentData(e, tr);
            transform.position = pos;
        };



    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        var buffer = manager.AddBuffer<WeaponItemComponent>(entity);
        buffer.Add
        (
            new WeaponItemComponent()
            {
                e = entity,
                active = active,
                weaponType = (int)weaponData.weaponType,
                special = special
            }
        );


        //manager.Add<WeaponItemComponent>(entity, new WeaponItemComponent
        //{
        //    active = active,
        //    weaponType = (int)weaponData.weaponType
        //});
        //manager.AddComponentData<WeaponItemComponent>(entity, new WeaponItemComponent
        //{
        //    active = active,
        //    weaponType = (int)weaponData.weaponType
        //});

    }


}
