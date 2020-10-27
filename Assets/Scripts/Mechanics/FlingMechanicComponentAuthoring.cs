using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct FlingMechanicComponent : IComponentData
{
    public bool active;
    public float force;
    public bool inFling;
    public float inFlingTime;
    public float inFlingMaxTime;
    public int counter;
    public bool vulnerable;
    public float vulnerableTime;
    public float vulnerableMaxTime;

}


[RequiresEntityConversion]
public class FlingMechanicComponentAuthoring: MonoBehaviour, IConvertGameObjectToEntity
{
    public bool active = true;
    public float force = 24f;
    public float inFlingMaxTime = 1.0f;
    public float vulnerableMaxTime = 2.0f;
    public ParticleSystem vulnerableParticleSystem;
    public ParticleSystem inFlingParticleSystem;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FlingMechanicComponent() {active = active, force = force, inFlingMaxTime = inFlingMaxTime, vulnerableMaxTime = vulnerableMaxTime});

    }
}
