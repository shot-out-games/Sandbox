using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TargetZones : MonoBehaviour, IConvertGameObjectToEntity
{

    public Transform headZone;
    [SerializeField]
    Transform deparentTriggers;
    public Entity TriggerEntity;

    void Awake()
    {
        if (deparentTriggers != null)
        {
            deparentTriggers.parent = null;
        }

        Animator animator = GetComponent<Animator>();
        if (!animator) return;
        if (headZone == null)
        {
            headZone = animator.GetBoneTransform(HumanBodyBones.Head);
        }


    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //TriggerEntity = conversionSystem.GetPrimaryEntity(TriggerGameObject);
        TriggerEntity = entity;
    }
}
