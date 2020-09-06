﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class ImpulseAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //dstManager.AddComponentData(entity, new LocalToWorld());
        //dstManager.AddComponentData(entity, new PhysicsVelocity());
        //dstManager.AddComponentData(entity, new PhysicsMass());
        dstManager.AddComponentData(entity, new ApplyImpulseComponent { Force = 0, Direction = Vector3.zero, Grounded = false });
    }
}