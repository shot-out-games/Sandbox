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


        Entities.ForEach((Entity e, ref DestroyComponent destroyComponent, ref Translation translation) =>
        {
            translation.Value = new float3(0, -50, 0);
            ecb.AddComponent(e, new DisableRendering());//not needed because it doesn't turn off child particle system render
            destroyComponent.frames++;
            if (destroyComponent.frames > 600)
            {
                ecb.DestroyEntity(e);
            }

        }).Schedule();


        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);


    }
}
