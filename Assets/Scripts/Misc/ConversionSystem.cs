using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;


public class ParticleConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        //Debug.Log("conversion");


        Entities.ForEach((ParticleSystem particleSystem, ParticleSystemRenderer particleSystemRenderer) =>
        //Entities.ForEach((ParticleSystem ps) =>
        {
            //AddHybridComponent(animator);
            //AddHybridComponent(particleSystem);
            //AddHybridComponent(particleSystemRenderer);
           // Debug.Log("ps " + particleSystem.name);
        });


        Entities.ForEach((AudioSource audioSource) =>
        {
            //Debug.Log("as " + audioSource);
            //AddHybridComponent(audioSource);
            //AddHybridComponent(particleSystemRenderer);

        });





    }
}

