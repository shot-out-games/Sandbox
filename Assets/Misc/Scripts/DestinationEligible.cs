using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public struct DestinationEligibleComponent : IComponentData
{
    public bool priority;
    //public bool completed;
    public int index;
    public Entity entity;

}

public class DestinationEligible : MonoBehaviour, IConvertGameObjectToEntity
{

    public bool priority;
    //private bool completed;
    public int index;
    public Entity entity;

    public EntityManager manager;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        manager = dstManager;
        manager.AddComponentData<DestinationEligibleComponent>(entity, new DestinationEligibleComponent()
        {
            entity = this.entity,
            //completed = false,
            priority = priority,
            index = index

        }
        );

    }
}