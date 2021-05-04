using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct FlingMechanicComponent : IComponentData
{
    public bool active;
    public bool flingInputEnabled;
    public float force;
    public bool inFling;
    public float inFlingTime;
    public float inFlingMaxTime;
    public int counter;
    public bool vulnerable;
    public float vulnerableTime;
    public float vulnerableMaxTime;
    public float vulnerableMaxTimeGame;
    public float timeSinceCausingDamage;
    public bool lastShotConnected;
    public bool shotLanded;
    public bool resetTimerAfterHitLanded;


}


public class FlingMechanicComponentAuthoring: MonoBehaviour, IConvertGameObjectToEntity
{
    public bool active = true;
    public bool flingInputEnabled = false;
    public float force = 24f;
    public float inFlingMaxTime = 1.0f;
    public float vulnerableMaxTime = 2.0f;
    [SerializeField] bool resetTimerAfterHitLanded;

    public ParticleSystem vulnerableParticleSystem;
    public ParticleSystem inFlingParticleSystem;
    public AudioSource flingAudioSource;
    public AudioClip flingAudioClip;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FlingMechanicComponent()
        {
            active = active, force = force, inFlingMaxTime = inFlingMaxTime, vulnerableMaxTime = vulnerableMaxTime, 
            flingInputEnabled = flingInputEnabled,
            vulnerableMaxTimeGame = vulnerableMaxTime,
            resetTimerAfterHitLanded = resetTimerAfterHitLanded

        });

    }
}
