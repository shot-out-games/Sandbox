using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;



public class EnemyWeaponMovementSystem :  SystemBase
{
    protected override void OnUpdate()
    {

        Entities.WithoutBurst().WithAll<EnemyComponent>().WithNone<Pause>().WithStructuralChanges().ForEach
        (
            (
                ref EnemyWeaponMovementComponent enemyMovementComponent,
                ref EnemyStateComponent enemyState,
                ref GunComponent gun,
                in Entity entity, in EnemyMove enemyMove, in Animator animator, in EnemyWeaponAim ammo
           ) =>
            {

                //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;
                if (enemyMovementComponent.enabled == false) return;




                if (enemyMove.target != null)
                {

                    enemyMove.speedMultiple = 1;
                    enemyMovementComponent.speedMultiple = 1;//set both because we set component but currently still need in MB

                    MoveStates MoveState = enemyState.MoveState;
                    EnemyRoles role = enemyMove.enemyRole;
                    //Vector3 enemyPosition = useStartPosition ? enemyMovementComponent.originalPosition : transform.position;
                    Vector3 enemyPosition = animator.transform.position;
                    Vector3 playerPosition = enemyMove.target.position;
                    float dist = Vector3.Distance(playerPosition, enemyPosition);
                    ammo.weaponRaised = false;
                    ammo.SetAnimationLayerWeights();

                    float dist_from_station = Vector3.Distance(enemyPosition, enemyMovementComponent.originalPosition);
                    bool useStartPosition = enemyMovementComponent.useDistanceFromStation;
                    float chaseRange = enemyMove.chaseRange;
                    if (useStartPosition == true && dist_from_station > chaseRange) chaseRange = -1;


                    ammo.weaponRaised = false;
                    gun.IsFiring = 0;


                    if (dist < enemyMove.shootRangeDistance)
                    {
                        ammo.weaponRaised = true;
                        //ammo.SetAnimationLayerWeights();
                        gun.IsFiring = 1;
                        MoveState = MoveStates.Idle;//need new state for when shooting then animation movement adjust to this
                        enemyMove.AnimationMovement();
                        enemyMove.FacePlayer();

                    }
                    else if (dist < chaseRange)
                    {
                        MoveState = MoveStates.Chase;
                        animator.SetInteger("Zone", 1);
                        enemyMove.SetDestination();
                        enemyMove.FacePlayer();

                    }
                    else if (dist >= chaseRange && role == EnemyRoles.Chase)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Idle;
                        enemyMove.AnimationMovement();
                        enemyMove.FaceWaypoint();
                    }
                    else if (dist >= chaseRange && role == EnemyRoles.Patrol)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Patrol;
                        enemyMove.Patrol();
                        enemyMove.FaceWaypoint();

                    }

                    ammo.SetAnimationLayerWeights();
                    enemyState = new EnemyStateComponent() { MoveState = MoveState };

                }





            }
        ).Run();
        //return default;
    }

}

