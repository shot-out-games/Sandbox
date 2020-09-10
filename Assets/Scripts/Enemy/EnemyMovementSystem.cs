using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;



public class EnemyMovementSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var entityManager = World.EntityManager;


        Entities.WithoutBurst().ForEach
        (
            (
                ref EnemyMovementComponent enemyMovementComponent,
                ref EnemyStateComponent enemyState,
                ref Translation translation,
                in Entity entity,
                in EnemyMove enemyMove,
                in Animator animator
            ) =>
            {

                if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;
                if (enemyMovementComponent.enabled == false) return;




                if (enemyMove.target != null)
                {
                    enemyMove.speedMultiple = 1;
                    enemyMovementComponent.speedMultiple =
                        1; //set both because we set component but currently still need in MB
                    float backupZoneFar = enemyMovementComponent.combatStrikeDistanceZoneEnd;
                    float backupZoneClose = enemyMovementComponent.combatStrikeDistanceZoneBegin;

                    MoveStates MoveState = enemyState.MoveState;
                    EnemyRoles enemyRole = enemyMove.enemyRole;
                    EnemyRoles role = enemyRole;
                    Vector3 playerPosition = enemyMove.target.position;
                    //bool useStartPosition = enemyMovementComponent.useDistanceFromStation;
                    //Vector3 enemyPosition = useStartPosition ? enemyMovementComponent.originalPosition : animator.transform.position;
                    Vector3 enemyPosition = animator.transform.position;
                    float dist = Vector3.Distance(playerPosition, enemyPosition);

                    if (dist < backupZoneClose)
                    {
                        MoveState = MoveStates.Default;
                        enemyMovementComponent.backup = true; //only time to turn on 
                        enemyMovementComponent.speedMultiple = dist / backupZoneClose;
                    }
                    else if (enemyMove.backup && dist > backupZoneFar)
                    {
                        MoveState = MoveStates.Default;
                        enemyMovementComponent.backup = false; //only time to turn off
                    }
                    else if (dist >= backupZoneClose && dist <= backupZoneFar)
                    {
                        MoveState = MoveStates.Default;
                        enemyMovementComponent.speedMultiple = (dist - backupZoneClose) / (backupZoneFar - backupZoneClose);

                        //enemyMovementComponent.speedMultiple = enemyMove.backup
                           // ? 1
                            //: (dist - backupZoneClose) / (backupZoneFar - backupZoneClose);
                    }

                    bool backup = enemyMovementComponent.backup;
                    float speedMultiple = enemyMovementComponent.speedMultiple;
                    enemyMove.backup = backup;
                    enemyMove.speedMultiple = speedMultiple;

                    float dist_from_station =
                        Vector3.Distance(enemyPosition, enemyMovementComponent.originalPosition);
                    bool useStartPosition = enemyMovementComponent.useDistanceFromStation;
                    float chaseRange = enemyMove.chaseRange;
                    if (useStartPosition == true && dist_from_station > chaseRange) chaseRange = -1;


                    if (backup)
                    {
                        MoveState = MoveStates.Default;
                        animator.SetInteger("Zone", 2);
                        Debug.Log("zone");
                        enemyMove.SetBackup();
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
                        enemyMove.AnimationMovement(); //dont chase just idle animation - sets animation param Z=0
                        enemyMove.FacePlayer();
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

                translation.Value.z = 0;





            }
        ).Run();




        Entities.WithoutBurst().ForEach(
              (
                  Entity e,
                  EnemyMove enemyMove,
                  in PhysicsVelocity physicsVelocity
              ) =>
              {
                  AudioSource audioSource = enemyMove.audioSource;

                  Vector3 velocity = enemyMove.agent.velocity;
                  float speed = velocity.magnitude;

                  if (speed >= .01f && math.abs(velocity.y) <= .000001f)
                  {
                      if (enemyMove.clip && audioSource)
                      {
                          audioSource.pitch = speed / 2f;
                          //Debug.Log("pitch n " + velocity.normalized);
                          //Debug.Log("pitch  mag " + velocity.normalized.magnitude);
                          if (audioSource.isPlaying == false)
                          {
                              audioSource.clip = enemyMove.clip;
                              audioSource.Play();
                                //Debug.Log("clip " + audioSource.clip);

                            }

                      }

                      if (enemyMove.ps)
                      {
                          if (enemyMove.ps.isPlaying == false)
                          {
                              enemyMove.ps.transform.SetParent(enemyMove.transform);
                              enemyMove.ps.Play(true);
                          }
                      }
                  }
                  else
                  {
                      if (audioSource != null) audioSource.Stop();
                      if (enemyMove.ps != null) enemyMove.ps.Stop();

                  }




              }
          ).Run();











        return default;
    }

}


