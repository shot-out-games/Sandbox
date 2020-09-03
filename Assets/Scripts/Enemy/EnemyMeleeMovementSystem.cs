using Unity.Entities;
using UnityEngine;
using Unity.Jobs;




public class EnemyMeleeMovementSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
        (
            ref EnemyMeleeMovementComponent enemyMovementComponent,
            ref EnemyStateComponent enemyState,
            in Entity entity,
            in RewindComponent rewindComponent,
            in Animator animator,
            in EnemyMove enemyMove


        ) =>
        {
            if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
            if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;
            if (enemyMovementComponent.enabled == false) return;


            if (rewindComponent.@on == true || rewindComponent.pressed == true)
            {
                enemyMove.SetDestination();
                Debug.Log("rew pressed");
            }
            else
            {
                if (enemyMove.target != null)
                {
                    enemyMove.speedMultiple = 1;
                    enemyMovementComponent.speedMultiple = 1;//set both because we set component but currently still need in MB
                    MoveStates MoveState = enemyState.MoveState;
                    EnemyRoles role = enemyMove.enemyRole;
                    //bool useStartPosition = enemyMovementComponent.useDistanceFromStation;
                    //Vector3 enemyPosition = useStartPosition ? enemyMovementComponent.originalPosition : animator.transform.position;
                    Vector3 enemyPosition = animator.transform.position;
                    float dist = Vector3.Distance(enemyMove.target.position, enemyPosition);
                    bool hasMelee = EntityManager.HasComponent(entity, typeof(EnemyMelee));
                    float backupZoneClose = hasMelee
                        ? animator.GetComponent<EnemyMelee>().strikeDistanceAdjustment
                        : enemyMovementComponent.combatStrikeDistanceZoneEnd;
                    float backupZoneFar = enemyMovementComponent.combatStrikeDistanceZoneEnd;
                    bool strike = false;
                    if (dist < backupZoneClose)
                    {
                        MoveState = MoveStates.Default;
                        enemyMovementComponent.backup = true;//only time to turn on 
                        enemyMovementComponent.speedMultiple = 1;
                    }
                    if (enemyMovementComponent.backup && dist > backupZoneFar)
                    {
                        MoveState = MoveStates.Default;
                        enemyMovementComponent.backup = false;//only time to turn off
                    }
                    else if (dist >= backupZoneClose && dist <= backupZoneFar)
                    {
                        enemyMovementComponent.speedMultiple = enemyMovementComponent.backup ? 1 : (dist - backupZoneClose) / (backupZoneFar - backupZoneClose);
                        MoveState = MoveStates.Default;
                        int n = Random.Range(0, 10);
                        if (enemyMovementComponent.backup == true && n <= 2)
                        {
                            strike = true;
                        }
                        else if (enemyMovementComponent.backup == false && n <= 5)
                        {
                            strike = true;
                        }
                    }

                    float dist_from_station = Vector3.Distance(enemyPosition, enemyMovementComponent.originalPosition);
                    bool useStartPosition = enemyMovementComponent.useDistanceFromStation;
                    float chaseRange = enemyMove.chaseRange;
                    if (useStartPosition == true && dist_from_station > chaseRange) chaseRange = -1;

                    bool backup = enemyMovementComponent.backup;
                    float speedMultiple = enemyMovementComponent.speedMultiple;
                    enemyMove.backup = backup;
                    enemyMove.speedMultiple = speedMultiple;
                    if (!hasMelee && dist <= backupZoneFar)
                    {
                        MoveState = MoveStates.Idle;
                        animator.SetInteger("Zone", 1);
                        enemyMove.SetDestination();
                        enemyMove.FacePlayer();

                    }
                    else if (backup)
                    {
                        MoveState = MoveStates.Default;
                        animator.SetInteger("Zone", 2);
                        enemyMove.SetDestination();
                        enemyMove.FacePlayer();

                    }
                    else if (strike)
                    {
                        MoveState = MoveStates.Default;
                        animator.SetInteger("Zone", 3);
                        enemyMove.SetDestination();
                        enemyMove.FacePlayer();

                    }
                    else if (dist < enemyMove.combatRangeDistance)
                    {
                        MoveState = MoveStates.Default;
                        animator.SetInteger("Zone", 2);
                        enemyMove.SetDestination();
                        enemyMove.FacePlayer();

                    }
                    else if (dist < chaseRange)
                    {
                        if (hasMelee) animator.GetComponent<EnemyMelee>().moveUsing.strikeDistanceAdjustment = 1;//reset when out of strike range
                        MoveState = MoveStates.Chase;
                        animator.SetInteger("Zone", 1);
                        enemyMove.SetDestination();
                        enemyMove.FacePlayer();

                    }
                    else if (dist >= chaseRange && role == EnemyRoles.Chase)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Idle;
                        enemyMove.AnimationMovement();//dont chase just idle animation - sets animation param Z=0
                        enemyMove.FaceWaypoint();

                    }
                    else if (dist >= chaseRange && role == EnemyRoles.Patrol)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Patrol;
                        enemyMove.Patrol();
                        enemyMove.FaceWaypoint();

                    }

                    enemyState = new EnemyStateComponent() { MoveState = MoveState };




                }
            }
        }
        ).Run();



        return default;
    }

}






