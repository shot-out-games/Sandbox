using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class ImpulseAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{


//    public float fallingFramesMax;
//    public float negativeForce = -9.81f;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
//        dstManager.AddComponentData(entity, new ApplyImpulseComponent { Force = 0, Direction = Vector3.zero, Grounded = false, fallingFramesMaximuim = fallingFramesMax, NegativeForce = negativeForce });
    }
}
