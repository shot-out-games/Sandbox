﻿using UnityEngine;
using Unity.Entities;

public class Trigger : MonoBehaviour, IConvertGameObjectToEntity
{

    public TriggerType Type;
    [SerializeField]
    private int index;





    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //parent needs to be fixed currently self
        TriggerComponent trigger = new TriggerComponent
        {
            Type = (int) Type, ParentEntity = conversionSystem.GetPrimaryEntity(transform.gameObject), CurrentFrame = 0, index = index
        };

        dstManager.AddComponentData(entity, trigger);
        if (Type == TriggerType.Ammo)
        {
            //Debug.Log("tr " + trigger.ParentEntity);
        }

    }


}