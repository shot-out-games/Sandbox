using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;

public class EnemyMeleeMovementSystem : SystemBase
{

    protected override void OnUpdate()
    {

        Entities.WithoutBurst().WithNone<Pause>().WithAll<EnemyComponent>().WithStructuralChanges().ForEach
        (
        (
            ref EnemyMeleeMovementComponent enemyMovementComponent,
            ref EnemyStateComponent enemyState,
            ref Translation translation,
            in Entity entity,
            in DeadComponent dead,
            in Animator animator,
            in EnemyMove enemyMove


        ) =>
        {
            if (dead.isDead) return;
            if (enemyMovementComponent.enabled == false) return;

            if (enemyMove.target != null)
            {
                enemyMove.speedMultiple = 1;
                MoveStates MoveState = enemyState.MoveState;
                EnemyRoles role = enemyMove.enemyRole;
                Vector3 enemyPosition = animator.transform.position;
                float dist = Vector3.Distance(enemyMove.target.position, enemyPosition);
                //float backupZoneClose = animator.GetComponent<EnemyMelee>().currentStrikeDistanceZoneBegin;
                float backupZoneClose = enemyMovementComponent.combatStrikeDistanceZoneBegin;
                float backupZoneFar = enemyMovementComponent.combatStrikeDistanceZoneEnd;
                bool strike = false;
                if (dist < backupZoneClose)
                {
                    MoveState = MoveStates.Default;
                    enemyMove.backup = true;//only time to turn on 
                    enemyMove.speedMultiple = dist / backupZoneClose;
                }

                if (enemyMove.backup && dist > backupZoneFar)
                {
                    MoveState = MoveStates.Default;
                    enemyMove.backup = false;//only time to turn off
                }
                else if (dist >= backupZoneClose && dist <= backupZoneFar)
                {
                    enemyMove.speedMultiple = (dist - backupZoneClose) / (backupZoneFar - backupZoneClose);
                    MoveState = MoveStates.Default;
                    int n = Random.Range(0, 10);
                    if (enemyMove.backup == true && n <= 2)
                    {
                        strike = true;
                    }
                    else if (enemyMove.backup == false && n <= 5)
                    {
                        strike = true;
                    }
                }

                float chaseRange = enemyMove.chaseRange;

                bool backup = enemyMove.backup;

                if (strike)
                {
                    MoveState = MoveStates.Default;
                    animator.SetInteger("Zone", 3);
                    enemyMove.SetDestination();
                    enemyMove.FacePlayer();

                }
                else if (backup)
                {
                    MoveState = MoveStates.Default;
                    animator.SetInteger("Zone", 2);
                    enemyMove.SetBackup();
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
                    animator.GetComponent<EnemyMelee>().currentStrikeDistanceAdjustment = 1;//reset when out of strike range
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
        ).Run();

    }

}






