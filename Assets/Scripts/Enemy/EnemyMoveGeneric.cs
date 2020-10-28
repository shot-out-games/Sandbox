using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;


public struct EnemyMoveGenericComponent : IComponentData
{


}




public class EnemyMoveGeneric : MonoBehaviour, IConvertGameObjectToEntity
{
    [HideInInspector]
    public NavMeshAgent agent;
    public float chaseRange;


    public float moveSpeed;
    public float rotateSpeed = 1;

    public Transform target;//default chase target but if combat used gets replaced by combatsystem move target
    public Entity entity;
    private EntityManager manager;
    private EnemyRatings enemyRatings;

    

    
    public AudioSource audioSource;
    public AudioClip clip;
    public ParticleSystem ps;



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




        if (agent)
        {
            //   agent.autoBraking = false;
            //agent.updateRotation = false;
            //agent.autoTraverseOffMeshLink = false;
                //agent.updatePosition = false;
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
            //agent.destination = nextPosition;
            agent.SetDestination(target.position);
            Debug.Log("nav  " + agent.speed);
        }
    }





    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entity = _entity;
        dstManager.AddComponentData(entity, new EnemyMoveGenericComponent());
        manager = dstManager;
        Init();
    }


}




public class EnemyMoveGenericSystem : SystemBase
{

    protected override void OnUpdate()
    {


        Entities.WithoutBurst().WithNone<Pause>().WithStructuralChanges().WithAll<EnemyMoveGenericComponent, EnemyComponent>().ForEach((Entity e, EnemyMoveGeneric move,
            ref Translation translation,
            in DeadComponent dead
            //in Pause pause
        ) =>
        {
            //if (pause.value == 1) return;
            if (dead.isDead) return;
            move.SetDestination();
            translation.Value.y = 0;//no jump for 1d jam
            //Debug.Log("en");



        }).Run();
        }
    }