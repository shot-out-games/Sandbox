using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public struct TrackerComponent : IComponentData
{
    public float3 Position;
    public Quaternion Rotation;

}

public enum TriggerType
{
    None = 0,
    Body = 1,
    Head = 2,
    Base = 3,//chest for player
    LeftHand = 4,
    RightHand = 5,
    LeftFoot = 6,
    RightFoot = 7,
    Ammo = 8,
    LevelItems = 9,
    Blocks = 10,
    PowerupHealth = 11,
    PowerupControl = 12,
    Ground = 13,
    Trigger = 14,
    Lever = 15,
    Key = 16,
    Home = 17,
    Melee = 18,
    Obstacle = 19,
    Contact = 20,
    Platform = 21
}



public class Tracker : MonoBehaviour, IConvertGameObjectToEntity
{

    public GameObject track;
    public Entity e;
    public Entity parentEntity;
    public EntityManager manager;
    public TriggerType Type;



    void Update()
    {
        if (manager == default) return;

        if (!manager.HasComponent(e, typeof(TrackerComponent))) return;
        if (track == null) return;

        var tracker = manager.GetComponentData<TrackerComponent>(e);
        //tracker.Position = track.transform.position;
        //tracker.Rotation = track.transform.rotation;
        manager.SetComponentData(e, tracker);

    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        parentEntity = conversionSystem.GetPrimaryEntity(track.transform.root.gameObject);
        //Debug.Log("parent " + parentEntity.Index);
        manager = dstManager;
        e = entity;
        manager.AddComponentData(entity, new TriggerComponent()
        {
            Type = (int)Type, 
            ParentEntity = parentEntity, CurrentFrame = 0,
            Entity = e


        });

        manager.AddComponent<TrackerComponent>(e);

    }


}


