using Cinemachine;
using Unity.Entities;
using UnityEngine;



public struct ImpulseComponent : IComponentData
{
    public float timer;
    public float maxTime;
    public bool activate;
}

public class Impulse : MonoBehaviour, IConvertGameObjectToEntity
{

    public CinemachineImpulseSource impulseSourceHitReceived;
    public CinemachineImpulseSource impulseSourceHitLanded;
    private Entity e;
    private EntityManager manager;
    Animator animator;
    [SerializeField]
    public float maxTime = 1.5f;


    void Awake()
    {

        //impulseSource = GetComponent<CinemachineImpulseSource>();
        animator = GetComponent<Animator>();


    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        dstManager.AddComponentData<ImpulseComponent>(e, new ImpulseComponent { maxTime = maxTime });

        //conversionSystem.AddHybridComponent(impulseSource);

    }
}
