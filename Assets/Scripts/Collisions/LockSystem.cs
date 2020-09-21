using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


public struct LockSystemComponent : IComponentData
{
    public float3 position;
    public bool justY;
}


//[UpdateAfter(typeof(Unity.Physics.Systems.EndFramePhysicsSystem))]

public class LockSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.



        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity e,  ref LockSystemComponent lockSystemComponent,
            ref Translation translation, ref PhysicsVelocity pv, ref Rotation rotation) =>
        {
            EntityManager.RemoveComponent<PhysicsVelocity>(e);

        }).Run();
    }
}
