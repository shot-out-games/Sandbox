using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections;



public enum WayPointAction
{
    Move,
    Jump,
    Idle
}


[System.Serializable]
public class WayPoint
{
    public Vector3 targetPosition;
    public Vector3 offset;
    public WayPointAction action;

}


public struct EnemyStateComponent : IComponentData
{
    public MoveStates MoveState;
    public CombatStates CombatState;

}

public enum MoveStates
{
    Default,
    Idle,
    Patrol,
    Chase,
    Defensive
};

public enum CombatStates
{
    Default,
    Idle,
    Chase,
    Stance,
    Aim
};

public enum EnemyRoles
{
    None,
    Chase,
    Patrol,
    Security,//removes all but first waypoint
    Evade,
    Random
};

public enum DefensiveRoles
{
    None,
    Chase,
    Patrol,
    Evade,
    Random
};

public enum NavigationStates
{
    Default,
    Movement,
    Melee,
    Weapon
};



public enum AttackStages
{
    No,
    Start,
    Action,
    End
}

public struct NavMeshAgentComponent : IComponentData
{

}

public struct MeleeComponent : IComponentData
{
    public bool Available;
    public float hitPower;
    public float gameHitPower;
    public bool anyTouchDamage;
}
public struct EnemyAttackComponent : IComponentData
{
    public AttackStages AttackStage;

}




public class EnemyMove : MonoBehaviour, IConvertGameObjectToEntity
{
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public Animator anim;

    //private NavMeshPath path;
    //public float chaseRange;
    //public float combatRangeDistance;
    //public float shootRangeDistance;

    //[HideInInspector]
    public List<WayPoint> wayPoints = new List<WayPoint>();
    [SerializeField]
    private int currentWayPointIndex = 0;
    [SerializeField]
    private WayPoint currentWayPoint;
    //[SerializeField]
    //private bool isCurrentWayPointJump;

    public bool randomWayPoints = false;
    public EnemyRoles enemyRole;
    public float moveSpeed;
    public float rotateSpeed = 1;

    public Transform target;//default chase target but if combat used gets replaced by combat system move target
    public Entity entity;
    private EntityManager manager;
    private EnemyRatings enemyRatings;

    public float speedMultiple;
    public bool backup;

    [HideInInspector]
    public Vector3 originalPosition;


    public AudioSource audioSource;
    public AudioClip clip;
    public ParticleSystem ps;
    [HideInInspector]
    public ParticleSystem stunEffect;//used by freeze system

    [SerializeField]
    float duration = 3.0f;
    float normalizedTime = 0.0f;
    //[SerializeField]
    Vector3 startPos;
    //[SerializeField]
    Vector3 endPos;
    //bool jumpTrigger;
    //bool wayPointComplete;
    public AnimationCurve curve = new AnimationCurve();
    bool jumpLanded;

    void Init()
    {
        enemyRatings = GetComponent<EnemyRatings>();
        //chaseRange = 18;
        //combatRangeDistance = 6;
        //shootRangeDistance = 8;

        agent = GetComponent<NavMeshAgent>();
        moveSpeed = 3.5f;

        RatingsComponent ratings = manager.GetComponentData<RatingsComponent>(entity);
        if (enemyRatings)
        {
            //chaseRange = ratings.chaseRangeDistance;
            //combatRangeDistance = ratings.combatRangeDistance;
            //shootRangeDistance = ratings.shootRangeDistance;
            if (agent)
            {
                agent.speed = ratings.speed;
                moveSpeed = agent.speed;
            }

        }

        originalPosition = transform.position;
        anim = GetComponent<Animator>();

        if (agent)
        {
            manager.AddComponent<NavMeshAgentComponent>(entity);
            agent.autoBraking = false;
            agent.updateRotation = false;
            //agent.updatePosition = false;
            agent.autoTraverseOffMeshLink = false;
        }



    }


    void Start()
    {
        //path = new NavMeshPath();
        SetWaypoints(randomWayPoints);


    }

    public void SetWaypoints(bool _randomWayPoints)
    {
        //waypoint 0 is initial so no matter where located travels there then never used again
        //waypoint 1 is where it goes next then cycles to last waypoint back to waypoint 1
        if (enemyRole == EnemyRoles.Patrol && wayPoints.Count <= 1) enemyRole = EnemyRoles.Chase;//patrol requires 2 min waypoints if not chnage role to chase


        for (int i = 0; i < wayPoints.Count; i++)
        {
            WayPoint wayPoint = wayPoints[i];
            wayPoint.targetPosition = transform.position + wayPoint.offset;
            wayPoint.action = wayPoints[i].action;
            wayPoints[i] = wayPoint;
        }

        if(wayPoints.Count == 0)
        {
            wayPoints.Add(new WayPoint { targetPosition = transform.position, action = WayPointAction.Idle } );
        }


        if (agent == null || agent.enabled == false) return;

        startPos = agent.transform.position;


        agent.destination = wayPoints[0].targetPosition;//goes to first waypoint just once at start;

        currentWayPointIndex = 0;
        bool isCurrentWayPointJump = wayPoints[currentWayPointIndex].action == WayPointAction.Jump;
        if (isCurrentWayPointJump == true)
        {
            anim.SetInteger("JumpState", 1);
            normalizedTime = 0.0f;
            endPos = wayPoints[0].targetPosition + Vector3.up * agent.baseOffset;
        }

        currentWayPointIndex = 0;


    }

    public void Patrol()
    {


        if (wayPoints.Count == 0 || agent.enabled == false)
            return;

        if (wayPoints[currentWayPointIndex].action == WayPointAction.Idle)
        {
            agent.speed = 0;
            AnimationMovement();
            return;
        }

        bool isCurrentWayPointJump = wayPoints[currentWayPointIndex].action == WayPointAction.Jump;
        //float distance = .5f;
        float distance = isCurrentWayPointJump ? 1.0f : 1.0f;

        //if (jumpTrigger == true )
        //{
        //  anim.SetInteger("JumpState", 1);
        //            normalizedTime = 0.0f;
        //startPos = agent.transform.position;
        //endPos = wayPoints[currentWayPointIndex].targetPosition + Vector3.up * agent.baseOffset;
        //return;
        //}


        //if (currentWayPointIndex == 0) distance = math.INFINITY;
        anim.SetInteger("JumpState", 0);


        if (agent.pathPending == false && agent.remainingDistance <= distance && isCurrentWayPointJump == false)
        {

            //  jumpTrigger = false;
            //Debug.Log("reached  " + agent.transform.position + " " + currentWayPointIndex);
            currentWayPointIndex++;
            if (currentWayPointIndex >= wayPoints.Count) currentWayPointIndex = 0;
            agent.destination = wayPoints[currentWayPointIndex].targetPosition;
            if (wayPoints[currentWayPointIndex].action == WayPointAction.Jump)
            {
                startPos = agent.transform.position;
                anim.SetInteger("JumpState", 1);
                normalizedTime = 0.0f;
                endPos = wayPoints[currentWayPointIndex].targetPosition + Vector3.up * agent.baseOffset;
            }

        }
        else if (agent.pathPending == false && agent.remainingDistance <= distance && isCurrentWayPointJump == true
            && jumpLanded == true)
        {
            //Debug.Log("reached  " + agent.transform.position + " " + currentWayPointIndex);
            //anim.SetInteger("JumpState", 0);

            jumpLanded = false;
            currentWayPointIndex++;
            if (currentWayPointIndex >= wayPoints.Count) currentWayPointIndex = 0;
            agent.destination = wayPoints[currentWayPointIndex].targetPosition;

            if (wayPoints[currentWayPointIndex].action == WayPointAction.Jump)
            {
                startPos = agent.transform.position;
                //Debug.Log("InJump Start " + agent.transform.position);
                anim.SetInteger("JumpState", 1);
                normalizedTime = 0.0f;
                endPos = wayPoints[currentWayPointIndex].targetPosition + Vector3.up * agent.baseOffset;
            }


            //agent.destination = wayPoints[currentWayPointIndex].targetPosition;
            //if (wayPoints[currentWayPointIndex].action == WayPointAction.Jump) jumpTrigger = true;//start a new jump

        }


        isCurrentWayPointJump = wayPoints[currentWayPointIndex].action == WayPointAction.Jump;
        //Debug.Log("current pt " + currentWayPointIndex);
        //if (isCurrentWayPointJump == false)
        //{
        AnimationMovement();
        //}

    }



    void Curve()
    {



        if (normalizedTime < 1.0f)
        {
            //jumpTrigger = false;
            float yOffset = curve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            //Debug.Log("InJump ");
            anim.SetInteger("JumpState", 1);


        }
        else
        {
            //isCurrentWayPointJump = false;
            //anim.SetInteger("JumpState", 0);
            //anim.SetInteger("Zone", 0);
            //Debug.Log("jump landed");
            jumpLanded = true;
            anim.SetInteger("JumpState", 0);


        }



    }



    void OnDrawGizmos()
    {
        if (agent == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(agent.destination, .095f);
    }



    public void FaceWaypoint()
    {
        if (!agent.enabled) return;
        Vector3 lookDir = agent.destination - transform.position;
        lookDir.y = 0;
        if (lookDir.magnitude < .019f) return;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

    }






    public void FacePlayer()
    {
        if (!agent) return;


        if (!agent.enabled) return;

        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;
        if (lookDir.magnitude < .019f) return;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

    }


    public void SetBackup()
    {
        if (agent == null) return;

        if (agent.enabled)
        {
            Vector3 nextPosition = target.position;
            Vector3 offset = transform.forward * Time.deltaTime * moveSpeed * 2;//try * 1
            agent.Move(-offset);
            AnimationMovement();
        }
    }

    public void SetDestination()
    {
        if (agent == null) return;

        if (agent.enabled)
        {

          




            //Debug.Log("path " + _path);
            Vector3 nextPosition = target.position;
            agent.destination = nextPosition;
            AnimationMovement();
        }
    }






    public void AnimationMovement()
    {

        if (anim == null || agent.isOnNavMesh == false) return;



        if (agent && manager.HasComponent<Pause>(entity) == false)
        {

            if (backup == false)
            {
                agent.updatePosition = true;
            }


            bool nearEdge = false;
            if(manager.HasComponent<EnemyMovementComponent>(entity) == true)
            {
                nearEdge = manager.GetComponentData<EnemyMovementComponent>(entity).nearEdge;
            }

            bool notMoving = false;
            NavMeshPath path = new NavMeshPath();
            if (target && agent.CalculatePath(target.position, path))
            {
                if (path.status == NavMeshPathStatus.PathPartial)
                {
                    Debug.Log("Partial");
                    notMoving = true;
                }
            }


            MoveStates state = manager.GetComponentData<EnemyStateComponent>(entity).MoveState;
            int pursuitMode = anim.GetInteger("Zone");
            agent.speed = pursuitMode >= 2 ? moveSpeed : moveSpeed * 2;
            Vector3 forward =
                transform.InverseTransformDirection(transform.forward); //world to local so always local forward (0,0,1)


            //float velx = 0;
            float velz = forward.normalized.z;

            if (state == MoveStates.Idle || state == MoveStates.Defensive || nearEdge == true || notMoving == true)
            {
                agent.speed = 0;
                velz = 0;
            }
            else if (state == MoveStates.Patrol)
            {
                agent.speed = moveSpeed * .5f;
                velz = .5f;
            }

            velz = velz * speedMultiple;
            //Debug.Log("next " + math.distance(agent.nextPosition, transform.position));
            anim.SetFloat("velz", velz);
        }
        else
        {
            agent.speed = 0;
        }

    }


    void Update()
    {

        if (agent == null) return;
        if (manager.HasComponent<Pause>(entity) == true)
        {
            agent.speed = 0;
            anim.speed = 0;
            return;
        }

        if (wayPoints.Count <= currentWayPointIndex) return;
        //if (anim.GetFloat("velz") == 0) agent.speed = 0;
        anim.speed = 1;

        bool isCurrentWayPointJump = wayPoints[currentWayPointIndex].action == WayPointAction.Jump;
        if (isCurrentWayPointJump == false)
        {
            agent.updatePosition = true;
            transform.position = agent.nextPosition;
        }
        else if (isCurrentWayPointJump)
        {
            agent.updatePosition = false;
            //anim.SetInteger("JumpState", 1);
            Curve();
        }

    }


    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entity = _entity;
        dstManager.AddComponentData(entity, new EnemyStateComponent { MoveState = MoveStates.Default, CombatState = CombatStates.Default });
        manager = dstManager;
        Init();
    }


}


