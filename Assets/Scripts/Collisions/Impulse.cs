using Cinemachine;
using Unity.Entities;
using UnityEngine;



public struct ImpulseComponent : IComponentData
{
    public float timer;
    public float maxTime;
    public float animSpeedRatio;
    public bool activate;

    public float timerOnReceived;
    public float maxTimeOnReceived;
    public float animSpeedRatioOnReceived;
    public bool activateOnReceived;
}

public class Impulse : MonoBehaviour, IConvertGameObjectToEntity
{

    public CinemachineImpulseSource impulseSourceHitReceived;
    public CinemachineImpulseSource impulseSourceHitLanded;
    private Entity e;
    private EntityManager manager;
    Animator animator;
    public float maxTime = 1.0f;
    public float animSpeedRatio = .5f;

    public float maxTimeOnReceived = 1.0f;
    public float animSpeedRatioOnReceived = .5f;


    void Awake()
    {

        //impulseSource = GetComponent<CinemachineImpulseSource>();
        animator = GetComponent<Animator>();


    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        dstManager.AddComponentData<ImpulseComponent>(e, new ImpulseComponent { maxTime = maxTime, animSpeedRatio = animSpeedRatio, maxTimeOnReceived = maxTimeOnReceived, animSpeedRatioOnReceived = animSpeedRatioOnReceived });

        //conversionSystem.AddHybridComponent(impulseSource);

    }
}
