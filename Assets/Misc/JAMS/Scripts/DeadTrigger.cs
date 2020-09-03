using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;




public class DeadTrigger : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField]
    private string tag;


    public Entity hitEntity;
    private EntityManager manager;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == tag)
        {
            Debug.Log("tag");
            manager.AddComponentData<DamageComponent>(hitEntity, new DamageComponent { DamageLanded = 0, DamageReceived = 100 });

        }



    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        hitEntity = entity;
        manager = dstManager;

    }
}