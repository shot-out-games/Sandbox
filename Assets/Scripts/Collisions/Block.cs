using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BlockComponent : IComponentData // also use for chargingComponent currently
{
    public bool blocked;
}
public class Block : MonoBehaviour, IConvertGameObjectToEntity
{
    private Entity e;
    private EntityManager manager;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        dstManager.AddComponentData<BlockComponent>(e, new BlockComponent() { blocked = false });
        manager = dstManager;

    }
}
