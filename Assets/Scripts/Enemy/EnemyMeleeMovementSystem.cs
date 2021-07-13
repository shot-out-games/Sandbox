using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

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
            in EnemyMove enemyMove,
            in EnemyBehaviourComponent enemyBehaviourComponent


        ) =>
        {
            if (dead.isDead) return;
            if (enemyMovementComponent.enabled == false) return;

            enemyMove.speedMultiple = 1;
            MoveStates MoveState = enemyState.MoveState;
            EnemyRoles role = enemyMove.enemyRole;

            if (enemyMove.target != null)
            {

                Vector3 enemyPosition = animator.transform.position;
                Vector3 homePosition = enemyMove.originalPosition;
                bool stayHome = enemyBehaviourComponent.useDistanceFromStation;
                float dist = Vector3.Distance(enemyMove.target.position, enemyPosition);
                float distFromStation = Vector3.Distance(homePosition, enemyPosition);




                //float backupZoneClose = animator.GetComponent<EnemyMelee>().currentStrikeDistanceZoneBegin;
                float backupZoneClose = enemyMovementComponent.combatStrikeDistanceZoneBegin;
                float backupZoneFar = enemyMovementComponent.combatStrikeDistanceZoneEnd;
                float chaseRange = enemyMove.chaseRange;


                bool strike = false;
                if (dist < backupZoneClose)
                {
                    MoveState = MoveStates.Default;
                    enemyMove.backup = true;//only time to turn on 
                    enemyMove.speedMultiple = dist / backupZoneClose;//try zero
                    strike = true;
                }

                if (enemyMove.backup && dist > backupZoneFar)
                {
                    MoveState = MoveStates.Default;
                    enemyMove.backup = false;//only time to turn off
                }
                else if (dist >= backupZoneClose && dist <= backupZoneFar)
                {
                    enemyMove.speedMultiple = math.sqrt((dist - backupZoneClose) / (backupZoneFar - backupZoneClose));
                    enemyMove.speedMultiple = 1;
                    MoveState = MoveStates.Default;
                    int n = Random.Range(0, 10);
                    //if (enemyMove.backup == true && n <= 2)
                    //{
                        strike = true;
                        //enemyMove.backup = false;//try

                    //}
                    //else if (enemyMove.backup == false && n <= 5)
                    //{
                    //  strike = true;
                    //}


                    //try below
                    //if (n <= 5)
                    //{
                    //  strike = true;
                    //enemyMove.backup = false;
                    //}

                }


                bool backup = enemyMove.backup;
                if (stayHome && distFromStation > chaseRange) chaseRange = 0;


                if (strike && dist < chaseRange)
                {
                    MoveState = MoveStates.Default;
                    animator.SetInteger("Zone", 3);
                    //Debug.Log("zone 1 strike move");
                    enemyMove.SetDestination();
                    enemyMove.FacePlayer();

                }
                else if (backup && dist < chaseRange)
                {
                    MoveState = MoveStates.Default;
                    animator.SetInteger("Zone", 2);
                    enemyMove.SetBackup();
                    enemyMove.FacePlayer();

                }
                else if (dist < enemyMove.combatRangeDistance && dist < chaseRange)
                {
                    MoveState = MoveStates.Default;
                    //Debug.Log("zone 1 melee move");
                    animator.SetInteger("Zone", 2);
                    enemyMove.SetDestination();
                    enemyMove.FacePlayer();

                }
                else if (dist < chaseRange)
                {
                    animator.GetComponent<EnemyMelee>().currentStrikeDistanceAdjustment = 1;//reset when out of strike range
                    MoveState = MoveStates.Chase;
                    //Debug.Log("zone 1 melee");
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
            else if (role == EnemyRoles.Patrol)
            {
                animator.SetInteger("Zone", 1);
                MoveState = MoveStates.Patrol;
                enemyMove.Patrol();
                enemyMove.FaceWaypoint();

            }

            //enemyState = new EnemyStateComponent() { MoveState = MoveState };


        }
        ).Run();

    }

}






