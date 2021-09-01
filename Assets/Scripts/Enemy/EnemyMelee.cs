using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class EnemyMelee : MonoBehaviour, IConvertGameObjectToEntity, ICombat
{
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;
    //[HideInInspector]
    public Transform AimTransform;
    //[HideInInspector]
    public FullBodyBipedIK ik;
    //[HideInInspector]
    public AimIK aim;
    //[HideInInspector]
    public FABRIK limb;
    public Moves moveUsing = new Moves();
    public float currentStrikeDistanceZoneBegin;
    public float currentStrikeDistanceAdjustment;


    [SerializeField]
    private List<Moves> moveList = new List<Moves>();
    public CombatStats combatStats = new CombatStats();
    public bool hitLanded { get; set; }
    public bool hitReceived { get; set; }
    public AttackStages AttackStage { get; set; }
    //public float strikeDistanceAdjustment { get; set; } = 1.0f;
    public MovesManager movesInspector;
    public bool attackStarted { get; internal set; }
    private EntityManager entityManager;
    private Entity meleeEntity;
    public bool active = true;
    public float hitPower = 10f;
    public bool allEnemyCollisionsCauseDamage;

    void Start()
    {
        //if (!entityManager.HasComponent<EnemyMeleeMovementComponent>(meleeEntity)) return;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        EnemyMove em = GetComponent<EnemyMove>();
        agent = GetComponent<NavMeshAgent>();

        ik = GetComponent<FullBodyBipedIK>();
        if(!movesInspector) return;

        for (int i = 0; i < movesInspector.Moves.Count; i++)
        {
            Moves move = movesInspector.Moves[i];
            if (move.aimIk != null)
            {
                aim = move.aimIk;
                AimTransform = move.aimTransform;
                if (AimTransform == null)
                {
                    //Debug.Log("aim ik auto bone ENEMY " + aim.solver.bones.Length);
                    int boneCount = aim.solver.bones.Length;
                    AimTransform = aim.solver.bones[boneCount - 1].transform;
                }

                if (move.limbIk)
                {
                    limb = move.limbIk;
                }

                currentStrikeDistanceZoneBegin = move.CalculateStrikeDistanceFromPinPosition(transform);
                move.target = moveUsing.target;//default target assigned in system
                moveList.Add(move);
            }
        }
        SelectMove();

    }

    public void SelectMove()
    {
        //Debug.Log("sm");
        if (moveList.Count == 0) return;
        combatAction = Random.Range(1, moveList.Count + 1);
        moveUsing = moveList[combatAction - 1];
        aim = moveUsing.aimIk;
        limb = moveUsing.limbIk;
        AimTransform = moveUsing.aimTransform;
        if (aim != null)
        {
            if (AimTransform == null)
            {
                int boneCount = aim.solver.bones.Length;
                AimTransform = aim.solver.bones[boneCount - 1].transform;
            }
            moveUsing.aimTransform = AimTransform;
        }
        if (limb != null)
        {
            //
        }

        if (moveUsing.moveAudioSource && moveUsing.moveAudioClip && moveUsing.moveAudioSource.isPlaying == false)
        {
            moveUsing.moveAudioSource.clip = moveUsing.moveAudioClip;
            moveUsing.moveAudioSource.Play();
        }
        if (moveUsing.moveParticleSystem)
        {
            moveUsing.moveParticleSystem.Play(true);
        }




    }

    public int combatAction { get; set; }

    public void StartMove()
    {
        int animationIndex = (int)moveUsing.animationType;
        animator.SetInteger("CombatAction", animationIndex);
    }

    public void StartAgent()
    {
        agent.enabled = true;

    }

    public void StopAgent()
    {
        agent.enabled = false;

    }

    public void StartAimIK()
    {
        if (moveUsing.usingAim && aim)
        {
            aim.enabled = true;
        }
        if (moveUsing.usingLimb && limb)
        {
            limb.enabled = true;
        }
    }

    public void StopAimIK()
    {
        if (moveUsing.usingAim && aim)
        {
            aim.enabled = false;
        }
        if (moveUsing.usingLimb && limb)
        {
            limb.enabled = false;
        }
    }


    public void StartIK()
    {
        if (moveUsing.usingFbb && ik)
        {
            ik.enabled = true;
        }
    }



    public void StopIK()
    {
        if (moveUsing.usingFbb && ik)
        {

            ik.enabled = false;
        }
    }

    public void Aim()
    {

        float hitWeight = animator.GetFloat("HitWeight");

        if (limb != null)
        {
            if (moveUsing.usingLimb && limb.enabled)
            {

                limb.solver.IKPosition = moveUsing.target.position;
                limb.solver.IKPositionWeight = hitWeight * moveUsing.weight;
                limb.solver.target = moveUsing.target;
                //Debug.Log("mv " + moveUsing.target.position);
                limb.solver.Update();
                //Debug.Log("hw " + hitWeight);
            }
        }



        if (ik == null || aim == null || moveUsing.target == null) return;
        //if (aim == null || !ik.enabled || !aim.enabled) return;
        if (moveUsing.usingAim && aim.enabled && moveUsing.pin)
        {
            aim.solver.transform = AimTransform;
            aim.solver.transform.LookAt(moveUsing.pin.position);
            try
            {
                aim.solver.IKPosition = moveUsing.target.position;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            aim.solver.IKPositionWeight = hitWeight * moveUsing.weight;
            aim.solver.Update();
        }




        if (moveUsing.usingFbb && ik.enabled)
        {
            ik.solver.GetEffector(moveUsing.effector).position = moveUsing.target.position;
            ik.solver.GetEffector(moveUsing.effector).positionWeight = hitWeight * moveUsing.weight;
            ik.solver.Update();
        }

    }

    public void LateUpdateSystem()
    {
        //if (entityManager.HasComponent<EnemyMeleeMovementComponent>(meleeEntity) == false) return;
        if (entityManager == default) return;
        Aim();
    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager = dstManager;
        meleeEntity = entity;
        entityManager.AddComponentData(meleeEntity, new MeleeComponent
        {
            Available = active,
            hitPower = hitPower,
            gameHitPower = hitPower,
            anyTouchDamage = allEnemyCollisionsCauseDamage

        });
        entityManager.AddComponentData(entity, new EnemyAttackComponent());

    }

}




public class EnemyMeleeSystem : SystemBase
{

    protected override void OnUpdate()
    {



        Entities.WithStructuralChanges().WithAll<EnemyComponent>().WithoutBurst().ForEach
        (
            (
                ref EnemyAttackComponent enemyAttackComponent,
                ref CheckedComponent checkedComponent,
                in Entity entity,
                in EnemyMove enemyMove,
                in EnemyMelee enemyCombat,
                in Transform transform,
                in TargetZones enemyTargetZone

            ) =>
            {
                bool hasMeleeComponent = EntityManager.HasComponent<MeleeComponent>(entity);
                if (hasMeleeComponent && enemyMove.target != null)
                {
                    bool attackStarted = enemyCombat.attackStarted;
                    float strikeDistanceAdjustment = enemyCombat.currentStrikeDistanceAdjustment;
                    float strikeDistance = enemyCombat.currentStrikeDistanceZoneBegin;
                    float adjStrikeDistance = strikeDistance * strikeDistanceAdjustment;

                    bool hitLanded = enemyCombat.hitLanded;
                    Vector3 enemyPosition = transform.position;
                    Vector3 playerPosition = enemyMove.target.position;
                    float dist = Vector3.Distance(playerPosition, enemyPosition);



                    if (attackStarted && enemyAttackComponent.AttackStage == AttackStages.No)
                    {
                        enemyAttackComponent = new EnemyAttackComponent() { AttackStage = AttackStages.Start };
                        //trigger.triggerChecked = false;
                        enemyCombat.SelectMove();
                    }
                    else if (attackStarted && enemyAttackComponent.AttackStage == AttackStages.Start)
                    {
                        enemyAttackComponent = new EnemyAttackComponent() { AttackStage = AttackStages.Action };
                    }
                    else if (!attackStarted && enemyAttackComponent.AttackStage == AttackStages.Action)
                    {
                        enemyAttackComponent = new EnemyAttackComponent() { AttackStage = AttackStages.End };
                    }
                    else if (!attackStarted && enemyAttackComponent.AttackStage == AttackStages.End)
                    {

                        if (hitLanded && dist < adjStrikeDistance && dist > adjStrikeDistance / 1.02f)
                        {
                            enemyCombat.currentStrikeDistanceAdjustment = 1.02f;//reset to 1.02f
                        }
                        else if (!hitLanded && strikeDistanceAdjustment > strikeDistance * .8f) // strike distance is the calculated strike distance in Moves class table
                        {
                            enemyCombat.currentStrikeDistanceAdjustment *= .98f;
                        }
                        //Debug.Log("strike " + adjStrikeDistance);
                        enemyCombat.hitLanded = false;
                        enemyCombat.hitReceived = false;
                        checkedComponent.collisionChecked = false;
                        enemyAttackComponent = new EnemyAttackComponent() { AttackStage = AttackStages.No };
                    }

                }

            }
        ).Run();

        //return default;
    }



}




