using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct DestroyComponent : IComponentData
{
    public int frames;
}



[UpdateInGroup((typeof(PresentationSystemGroup)))]
public class DestroySystem : SystemBase
{

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;


    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }


    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();


        Entities.WithoutBurst().WithStructuralChanges().ForEach((Entity e, ref DestroyComponent destroyComponent, in PowerItem powerItem) =>
        {

            destroyComponent.frames++;
            if (destroyComponent.frames > 60)
            {
                ecb.DestroyEntity(e);
            }

        }).Run();


        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);


    }
}
