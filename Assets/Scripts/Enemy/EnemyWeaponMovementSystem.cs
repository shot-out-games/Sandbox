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
                in Entity entity, in DeadComponent dead,
                in EnemyMove enemyMove, in Animator animator, in EnemyWeaponAim ammo
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
                    Vector3 playerPosition = enemyMove.target.position;
                    float dist = Vector3.Distance(playerPosition, enemyPosition);
                    ammo.weaponRaised = false;
                    ammo.SetAnimationLayerWeights();
                    float chaseRange = enemyMove.chaseRange;
                    ammo.weaponRaised = false;
                    gun.IsFiring = 0;

                    if (dist < enemyMove.shootRangeDistance)
                    {
                        ammo.weaponRaised = true;
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
    }

}

