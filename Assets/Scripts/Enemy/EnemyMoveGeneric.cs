using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Random = Unity.Mathematics.Random;


public struct EnemyMoveGenericComponent : IComponentData
{
    public float3 startVelocity;

}




public class EnemyMoveGeneric : MonoBehaviour, IConvertGameObjectToEntity
{
    [HideInInspector]
    public NavMeshAgent agent;
    public float chaseRange;
    [SerializeField] private bool randomize = false;
    [SerializeField]
    Vector3 startVelocity = Vector3.zero;


    public float moveSpeed;
    public float rotateSpeed = 1;

    public Transform target;//default chase target but if combat used gets replaced by combatsystem move target
    public Entity entity;
    private EntityManager manager;
    private EnemyRatings enemyRatings;
    
    

    
    public AudioSource audioSource;
    public AudioClip clip;
    public ParticleSystem ps;

    void Awake()
    {
        if (randomize)
        {
            float max = startVelocity.z;
            startVelocity = float3.zero;
            startVelocity.z = UnityEngine.Random.Range(-max, max);
            startVelocity.x = UnityEngine.Random.Range(-max, max);
        }

    }


    void Init()
    {
        enemyRatings = GetComponent<EnemyRatings>();
        chaseRange = 18;
        agent = GetComponent<NavMeshAgent>();
        moveSpeed = 3.5f;

        RatingsComponent ratings = manager.GetComponentData<RatingsComponent>(entity);
        if (enemyRatings)
        {
            chaseRange = ratings.chaseRangeDistance;
            if (agent)
            {
                agent.speed = ratings.speed;
                moveSpeed = agent.speed;
            }

        }

        Debug.Log("nav  " + agent.speed);


        if (agent)
        {
            manager.AddComponent<NavMeshAgentComponent>(entity);
            agent.autoBraking = false;
            agent.updateRotation = false;
            agent.autoTraverseOffMeshLink = false;
        }



    }





    public void FacePlayer()
    {
        if(!agent) return;
        

        if (!agent.enabled) return;
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;
        if (lookDir.magnitude < .019f) return;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

    }


    public void SetDestination()
    {
        if (agent == null || manager == default || entity == Entity.Null) return;

        if (agent.enabled)
        {
            Vector3 nextPosition = target.position;
            agent.SetDestination(target.position);
        }
    }





    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entity = _entity;
        dstManager.AddComponentData(entity, new EnemyMoveGenericComponent() { startVelocity = startVelocity } );
        manager = dstManager;
        Init();
    }


}




public class EnemyMoveGenericSystem : SystemBase
{

    protected override void OnUpdate()
    {


        Entities.WithoutBurst().WithNone<Pause>().WithStructuralChanges().WithAll<EnemyComponent>().ForEach((Entity e, EnemyMoveGeneric move, 
            ref Translation translation, ref PhysicsVelocity pv,
            in DeadComponent dead,
            in EnemyMoveGenericComponent enemyMoveGenericComponent
        ) =>
        {
            if (dead.isDead) return;
            move.SetDestination();
            if(Vector3.SqrMagnitude(pv.Linear) < .0001f)
            {
                pv.Linear = enemyMoveGenericComponent.startVelocity;
            }
            translation.Value.y = 0;//no jump for 1d jam

        }).Run();
        }
    }