using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

// IConvertGameObjectToEntity pipeline is called *before* the Physics Body & Shape Conversion Systems
// This means that there would be no PhysicsMass component to tweak when Convert is called.
// Instead Convert is called from the PhysicsSamplesConversionSystem instead.
public class SetInertiaInverseBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool LockX = false;
    public bool LockY = false;
    public bool LockZ = false;

    public bool LockVelocity = false;


    private Entity e;
    private EntityManager manager;


    void Start()
    {
     //   position = transform.position;
    }

    void LateUpdate()
    {
        if(e == Entity.Null) return;

        if (manager.HasComponent<PhysicsMass>(e))
        {
            var mass = manager.GetComponentData<PhysicsMass>(e);
            //mass.InverseMass = .01f;
            mass.InverseInertia[0] = LockX ? .0f: mass.InverseInertia[0];
            mass.InverseInertia[1] = LockY ? .0f: mass.InverseInertia[1];
            mass.InverseInertia[2] = LockZ ? .0f : mass.InverseInertia[2]; 
            manager.SetComponentData<PhysicsMass>(e, mass);
        }

        //manager.RemoveComponent<PhysicsVelocity>(e);


        //if (manager.HasComponent<PhysicsVelocity>(e))
        //{
        //    var velocity = manager.GetComponentData<PhysicsVelocity>(e);
        //    velocity.Linear.x = LockX ? 0 : velocity.Linear.x;
        //    velocity.Linear.y = LockY ? 0 : velocity.Linear.y;
        //    velocity.Linear.z = LockZ ? 0 : velocity.Linear.z;
        //    manager.SetComponentData<PhysicsVelocity>(e, velocity);
        //}

        //transform.position = position;

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        if(LockVelocity) manager.AddComponent<LockSystemComponent>(e);
        //manager.RemoveComponent<PhysicsVelocity>(e);

    }
}

