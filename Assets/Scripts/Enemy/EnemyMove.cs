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
    Jump
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
    Chase
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


public struct MeleeComponent : IComponentData
{
    public bool Available;
    public float hitPower;
    public float gameHitPower;
}
public struct EnemyAttackComponent : IComponentData
{
    public AttackStages AttackStage;

}




public class EnemyMove : MonoBehaviour, IConvertGameObjectToEntity
{
    [HideInInspector]
    public NavMeshAgent agent;
    private Animator anim;

    public float chaseRange;
    public float combatRangeDistance;
    public float shootRangeDistance;

    //[HideInInspector]
    public List<WayPoint> wayPoints = new List<WayPoint>();
    [SerializeField]
    private int currentWayPointIndex = 0;
    [SerializeField]
    private WayPoint currentWayPoint;
    [SerializeField]
    private bool isCurrentWayPointJump;

    public bool randomWayPoints = false;
    public EnemyRoles enemyRole;
    public float moveSpeed;
    public float rotateSpeed = 1;
    public Transform target;//default chase target but if combat used gets replaced by combatsystem move target
    public Entity entity;
    private EntityManager manager;
    private EnemyRatings enemyRatings;

    public float speedMultiple;
    public bool backup;

    [SerializeField]
    private Vector3 originalPosition;
    private Vector3 destinationBeforeRewind;
    private bool currentRewind;


    public AudioSource audioSource;
    public AudioClip clip;
    public ParticleSystem ps;

    public ParticleSystem stunEffect;




    //[SerializeField]
    //Vector3[] offsets;
    [SerializeField]
    Vector3 leapTarget;
    [SerializeField]
    float duration = 3.0f;
    float normalizedTime = 0.0f;
    Vector3 startPos;
    Vector3 endPos;



    public AnimationCurve curve = new AnimationCurve();


    void Init()
    {
        enemyRatings = GetComponent<EnemyRatings>();
        chaseRange = 18;
        combatRangeDistance = 6;
        shootRangeDistance = 8;

        agent = GetComponent<NavMeshAgent>();
        moveSpeed = 3.5f;

        RatingsComponent ratings = manager.GetComponentData<RatingsComponent>(entity);
        if (enemyRatings)
        {
            chaseRange = ratings.chaseRangeDistance;
            combatRangeDistance = ratings.combatRangeDistance;
            shootRangeDistance = ratings.shootRangeDistance;
            agent.speed = ratings.speed;
            moveSpeed = agent.speed;

        }



        agent.autoBraking = false;
        anim = GetComponent<Animator>();
        //SetWaypoints(randomWayPoints);
        agent.updateRotation = false;
        originalPosition = transform.position;


    }


    void Start()
    {
        SetWaypoints(randomWayPoints);

    }

    public void SetWaypoints(bool _randomWayPoints)
    {


        for (int i = 0; i < wayPoints.Count; i++)
        {
            WayPoint wayPoint = wayPoints[i];
            wayPoint.targetPosition = transform.position + wayPoint.offset;
            wayPoint.action = wayPoints[i].action;
            wayPoints[i] = wayPoint;
        }

    }

    public void Patrol()
    {
        //        if (wayPoints.Count == 0)
        //     {
        //      SetWaypoints(randomWayPoints);
        //}


        if (wayPoints.Count == 0 | agent.enabled == false)
            return;

        //Debug.Log("path pending " + agent.pathPending + " dist " + agent.remainingDistance);

        //Debug.Log("path");


        float distance = isCurrentWayPointJump ? .0003f : .5f;

        if (agent.pathPending == false && agent.remainingDistance < distance)
        {
            //wayPoints[currentWayPoint] = transform.position + offsets[currentWayPoint];
            anim.SetInteger("JumpState", 0);
            agent.destination = wayPoints[currentWayPointIndex].targetPosition;
            isCurrentWayPointJump = wayPoints[currentWayPointIndex].action == WayPointAction.Jump;
            //leapTarget = wayPoints[currentWayPointIndex].targetPosition;

            if (isCurrentWayPointJump)
            {
                //StartCoroutine(Curve(agent, duration));
                anim.SetInteger("JumpState", 1);
                normalizedTime = 0.0f;
                startPos = agent.transform.position;
                endPos = wayPoints[currentWayPointIndex].targetPosition + Vector3.up * agent.baseOffset;
            }


            currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Count;


        }




        //        else
        //     {

        if (isCurrentWayPointJump == false)
        {
            AnimationMovement();
        }
        //  }


    }


    public void SetDestination()
    {
        if (agent == null || manager == null || entity == Entity.Null) return;

        bool noX = false;
        bool noZ = true;

        if (agent.enabled)
        {

            Vector3 nextPosition = target.position;
            if (noZ) nextPosition.z = 0;
            agent.destination = nextPosition;
            AnimationMovement();
        }
    }




    void Curve()
    {

        if (normalizedTime < 1.0f)
        {

            //Debug.Log("next0 " + agent.nextPosition);
            float yOffset = curve.Evaluate(normalizedTime);
            //Debug.Log("y " + math.round(yOffset));
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
                //Debug.Log("n " + normalizedTime);
            //Debug.Log("pos " + agent.transform.position);

            // yield return null;

        }
        else
        {
            isCurrentWayPointJump = false;
            anim.SetInteger("JumpState", 0);

        }

    }



    void OnDrawGizmos()
    {
        if (agent == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(agent.destination, 1);
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
        if (!agent.enabled) return;
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;
        if (lookDir.magnitude < .019f) return;
        Quaternion rot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);

    }

    public void AnimationMovement()
    {

        if (target == null || anim == null) return;


        //Debug.Log("next 1 " + agent.nextPosition);
        //Debug.Log("rem 1 " + agent.remainingDistance);


        //bool onLink = agent.isOnOffMeshLink;




        MoveStates state = manager.GetComponentData<EnemyStateComponent>(entity).MoveState;
        int pursuitMode = anim.GetInteger("Zone");
        agent.speed = pursuitMode >= 2 ? moveSpeed : moveSpeed * 2;
        Vector3 forward =
            transform.InverseTransformDirection(transform.forward); //world to local so always local forward (0,0,1)


        float velx = 0;
        float velz = forward.normalized.z;

        if (currentRewind == true)
        {
            agent.speed = moveSpeed * 2;
            velz = velz * 2;
        }
        else if (state == MoveStates.Idle)
        {
            agent.speed = 0;
            velz = 0;
        }
        else if (state == MoveStates.Patrol)
        {
            //agent.speed = agent.speed * .5f;
            agent.speed = moveSpeed * .5f;
            velz = .5f;
        }



        velz = velz * speedMultiple;

        //Debug.Log("z " + velz);

        anim.SetFloat("velx", velx);
        anim.SetFloat("velz", velz);



    }


    void OnAnimatorMove()
    {

        //return;

        if (agent == null || manager == null || entity == Entity.Null) return;
        if (isCurrentWayPointJump == false)
        {

            float speed = speedMultiple * 1.0f;
            Vector3 velocity = anim.deltaPosition / Time.deltaTime * speed;

            //Debug.Log("v " + velocity + " " + speed);
            //Debug.Log("v " + agent.velocity);

            if (backup)
            {
                agent.velocity = -velocity * .5f;
            }
            else
            {
                transform.position = agent.nextPosition;
            }


            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        else if (isCurrentWayPointJump)
        {
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


