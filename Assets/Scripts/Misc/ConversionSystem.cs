using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class ParticleConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ParticleSystem particleSystem, ParticleSystemRenderer particleSystemRenderer) =>
        {
            AddHybridComponent(particleSystem);
            AddHybridComponent(particleSystemRenderer);

        });
    }
}

