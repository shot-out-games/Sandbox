using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


public enum TriggerType
{
    None = 0,
    Body = 1,
    Head = 2,
    Chest = 3,
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
    Home = 17
}

[RequiresEntityConversion]


public class Tracker : MonoBehaviour, IConvertGameObjectToEntity
{

    public GameObject track;
    public Entity e;
    public Entity parentEntity;
    public EntityManager manager;
    public TriggerType Type;
    [SerializeField] private Transform offset;//since we can't make child need position relative to tracked transform
                                              //    private Vector3 offsetPosition = Vector3.zero;
                                              // private Quaternion offsetRotation = Quaternion.identity;


    //[SerializeField] private Vector3 transformPoint;//since we can't make child need position relative to tracked transform
    //[SerializeField] private GameObject prefab;
    //private GameObject go;

    //void Start()
    //{

    //    if(offset != null)
    //    {
    //        offsetPosition = offset.localPosition;
    //        offsetRotation = offset.localRotation;
    //    }

    //}

    //void LateUpdate()
    //{
    //    if (manager == null) return;

    //    if (!manager.HasComponent(e, typeof(Translation))) return;
    //    if (track == null) return;

    //    transform.position = track.transform.position + offsetPosition;
    //    transform.rotation = track.transform.rotation * offsetRotation;

    //    manager.SetComponentData(e, new Translation { Value = transform.position });
    //    manager.SetComponentData(e, new Rotation { Value = transform.rotation });

    //}

    void LateUpdate()
    {
        if (manager == null) return;

        if (!manager.HasComponent(e, typeof(TriggerComponent))) return;
        if (track == null) return;

        var trigger = manager.GetComponentData<TriggerComponent>(e);
        ////track.transform.position = trigger.Position;
        ////track.transform.rotation = trigger.Rotation;
        trigger.Position = track.transform.position;
        trigger.Rotation = track.transform.rotation;
        ////Debug.Log("pos mb " + track.transform.position);
        //Debug.Log("pos mb ");
        manager.SetComponentData(e, trigger);

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

    }


}


