using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using Unity.Jobs;



public struct NpcStateComponent : IComponentData
{
    public MoveStates MoveState;
    public CombatStates CombatState;

}


public enum NpcRoles
{
    Chase,
    Patrol,
    Evade,
    Random
};



public struct NpcMovementComponent : IComponentData
{
    public float speedMultiple;
    public float speed;
    public float combatStrikeDistanceZoneBegin;
    public float combatStrikeDistanceZoneEnd;
    public float combatRangeDistance;
    public float chaseRange;
    public float aggression;
    public float maxHealth;
    public bool backup;
    public bool chaseOnly;

}


public struct NpcAttackComponent : IComponentData
{
    public AttackStages AttackStage;

}

public class NonPlayerCharacter : MonoBehaviour, IConvertGameObjectToEntity
{
    [HideInInspector]
    public NavMeshAgent agent;
    private Animator anim;

    public float chaseRange;
    public float combatRangeDistance;
    public float shootRangeDistance = 8.0f;
    public float targetOffset { set; get; } = 0.0f;
    //public bool backup;

    [HideInInspector] public List<Vector3> wayPoints = new List<Vector3>();
    private int currentWayPoint = 0;
    public bool randomWayPoints = false;
    public NpcRoles npcRole;
    public float moveSpeed;
    public float rotateSpeed = 1;
    public Transform target;
    public Entity entity;
    private EntityManager manager;

    private NpcRatings npcRatings;

    void Init()
    {
        npcRatings = GetComponent<NpcRatings>();
        chaseRange = 18;
        combatRangeDistance = 6;


        agent = GetComponent<NavMeshAgent>();
        moveSpeed = 3.5f;

        if (npcRatings)
        {
           // Debug.Log("npc ratings " + entity);
            chaseRange = npcRatings.Ratings.chaseRange;
            combatRangeDistance = npcRatings.Ratings.combatRangeDistance;
            agent.speed = npcRatings.Ratings.speed;
            moveSpeed = agent.speed;

        }

        agent.autoBraking = false;
        anim = GetComponent<Animator>();
        SetWaypoints(randomWayPoints);
        agent.updateRotation = false;

        //gameObject.SetActive(false);

    }


    public void SetWaypoints(bool _randomWayPoints)
    {
        Vector3[] offsets = new[]
            {new Vector3(10, 0, 0), new Vector3(10, 0, 10), new Vector3(-10, 0, 0), new Vector3(0, 0, 0)};


        for (int i = 0; i < offsets.Length; i++)
        {
            wayPoints.Add(transform.position + offsets[i]);
        }

    }

    public void Patrol()
    {
        if (wayPoints.Count == 0)
            return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.destination = wayPoints[currentWayPoint];
            currentWayPoint = (currentWayPoint + 1) % wayPoints.Count;
        }
        AnimationMovement();

    }


    public void SetDestination()
    {
        if (target == null || anim == null ||  entity == Entity.Null || manager == null)  return;
        if (manager.HasComponent<NpcComponent>(entity) == false) return;

        if (agent.enabled && manager.GetComponentData<NpcComponent>(entity).active == true )
        {
            Vector3 offset = transform.TransformDirection(new Vector3(0, 0, targetOffset));//not used yet
            if (agent.enabled)
            {
                agent.destination = target.position + offset;
            }
            AnimationMovement();

        }

    }

    void OnDrawGizmos()
    {
        if (agent == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(agent.destination, 1);
    }



    public void FacePlayer()
    {
        if (!agent.enabled) return;
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

    }

    public void AnimationMovement()
    {

        if (target == null || anim == null ||  entity == Entity.Null || manager == null) return;
        if (manager.HasComponent<NpcComponent>(entity) == false) return;
        if (manager.GetComponentData<NpcComponent>(entity).active == false) return;

        MoveStates state = manager.GetComponentData<NpcStateComponent>(entity).MoveState;
        int pursuitMode = anim.GetInteger("Zone");
        agent.speed = pursuitMode >= 2 ? moveSpeed : moveSpeed * 2;
        Vector3 forward = transform.InverseTransformDirection(transform.forward);//world to local so always local forward (0,0,1)


        float velx = 0;
        float velz = forward.normalized.z;

        if (state == MoveStates.Idle)
        {
            agent.speed = 0;
            velz = 0;
        }
        else if (state == MoveStates.Patrol)
        {
            agent.speed = agent.speed * .5f;
            velz = .5f;
        }
        velz = velz * manager.GetComponentData<NpcMovementComponent>(entity).speedMultiple;

        anim.SetFloat("velx", velx);
        anim.SetFloat("velz", velz, .06f, Time.deltaTime);

    }


    void OnAnimatorMove()
    {
        if (target == null || anim == null ||  entity == Entity.Null || manager == default) return;
        bool hasComponent = manager.HasComponent(entity, typeof(NpcMovementComponent));
        if (manager.HasComponent<NpcComponent>(entity) == false) return;
        if (hasComponent == false) return;
        if (manager.GetComponentData<NpcComponent>(entity).active == false) return;



        float speed = manager.GetComponentData<NpcMovementComponent>(entity).speedMultiple;
        bool backup = manager.GetComponentData<NpcMovementComponent>(entity).backup;

        if (backup)
        {
            Vector3 velocity = anim.deltaPosition / Time.deltaTime * speed;
            agent.velocity = -velocity;

        }
        else
        {
            transform.position = agent.nextPosition;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" 
            || collision.gameObject.tag == "NPC")
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = false;

    }








    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entity = _entity;
        dstManager.AddComponentData(entity, new NpcStateComponent { MoveState = MoveStates.Default, CombatState = CombatStates.Default });
        manager = dstManager;
        //Debug.Log("npc convert ");
        Init();

    }


}




