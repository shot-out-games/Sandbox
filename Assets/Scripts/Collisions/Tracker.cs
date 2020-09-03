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
    Key = 16
}

[RequiresEntityConversion]


public class Tracker : MonoBehaviour, IConvertGameObjectToEntity
{

    public GameObject track;
    public Entity _Entity;
    public Entity parentEntity;
    public EntityManager _EntityManager;
    public TriggerType Type;
    [SerializeField] private Transform offset;//since we can't make child need position relative to tracked transform
    private Vector3 offsetPosition = Vector3.zero;
    private Quaternion offsetRotation = Quaternion.identity;


    //[SerializeField] private Vector3 transformPoint;//since we can't make child need position relative to tracked transform
    //[SerializeField] private GameObject prefab;
    //private GameObject go;

    void Start()
    {

        if(offset != null)
        {
            offsetPosition = offset.localPosition;
            offsetRotation = offset.localRotation;
        }

    }

    void LateUpdate()
    {
        if (_EntityManager == null) return;

        if (!_EntityManager.HasComponent(_Entity, typeof(Translation))) return;
        if(track == null) return;

        transform.position = track.transform.position + offsetPosition;
        transform.rotation = track.transform.rotation * offsetRotation;

        _EntityManager.SetComponentData(_Entity, new Translation { Value = transform.position });
        _EntityManager.SetComponentData(_Entity, new Rotation  { Value = transform.rotation });

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        parentEntity = conversionSystem.GetPrimaryEntity(track.transform.root.gameObject);
        //Debug.Log("parent " + parentEntity.Index);
        _EntityManager = dstManager;
        _Entity = entity;
        _EntityManager.AddComponentData(entity, new TriggerComponent() { Type = (int)Type, ParentEntity = parentEntity, CurrentFrame = 0 });

    }


}


