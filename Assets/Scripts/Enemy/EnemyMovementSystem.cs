//using System.Collections;
//using System.Collections.Generic;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;



//public class EnemyMovementSystem : SystemBase
//{

//    protected override void OnUpdate()
//    {


//        Entities.WithAll<EnemyComponent>().WithNone<Pause>().WithoutBurst().ForEach
//        (
//            (
//                ref EnemyMovementComponent enemyMovementComponent,
//                ref EnemyStateComponent enemyState,
//                ref Translation translation,
//                in Entity entity,
//                in DeadComponent dead,
//                in EnemyMove enemyMove,
//                in Animator animator
//            ) =>
//            {

//                if (dead.isDead) return;
//                if (enemyMovementComponent.enabled == false) return;

//                if (enemyMove.target != null && enemyMove.enemyRole != EnemyRoles.None)
//                {
//                    enemyMove.speedMultiple = 1;
//                    MoveStates MoveState = enemyState.MoveState;
//                    EnemyRoles enemyRole = enemyMove.enemyRole;
//                    EnemyRoles role = enemyRole;
//                    Vector3 playerPosition = enemyMove.target.position;
//                    Vector3 enemyPosition = animator.transform.position;
//                    float dist = Vector3.Distance(playerPosition, enemyPosition);
//                    float chaseRange = enemyMove.chaseRange;
//                    if (dist < chaseRange)
//                    {
//                        MoveState = MoveStates.Chase;
//                        animator.SetInteger("Zone", 1);
//                        Debug.Log("zone 1");
//                        enemyMove.SetDestination();
//                        enemyMove.FacePlayer();
//                    }
//                    else if (dist >= chaseRange && role == EnemyRoles.Chase)
//                    {
//                        animator.SetInteger("Zone", 1);
//                        MoveState = MoveStates.Idle;
//                        enemyMove.AnimationMovement(); //dont chase just idle animation - sets animation param Z=0
//                        enemyMove.FaceWaypoint();
//                    }
//                    else if (dist >= chaseRange && role == EnemyRoles.Patrol)
//                    {
//                        Debug.Log("patrol");
//                        animator.SetInteger("Zone", 1);
//                        MoveState = MoveStates.Patrol;
//                        enemyMove.Patrol();
//                        enemyMove.FaceWaypoint();
//                    }
//                    enemyState = new EnemyStateComponent() { MoveState = MoveState };
//                    //translation.Value.z = 0;
//                }
//                //translation.Value.y = 0;//only if no jumping enemy temp
//            }
//        ).Run();


//        Entities.WithoutBurst().WithAll<EnemyComponent>().ForEach(
//              (
//                  Entity e,
//                  EnemyMove enemyMove,
//                  in PhysicsVelocity physicsVelocity
//              ) =>
//              {
//                  AudioSource audioSource = enemyMove.audioSource;
//                  Vector3 velocity = Vector3.zero;

//                  if (enemyMove.agent)
//                  {
//                      velocity = enemyMove.agent.velocity;
//                  }

//                  float speed = velocity.magnitude;

//                  if (speed >= .01f && math.abs(velocity.y) <= .000001f)
//                  {
//                      if (enemyMove.clip && audioSource)
//                      {
//                          audioSource.pitch = speed / 2f;
//                          //Debug.Log("pitch n " + velocity.normalized);
//                          //Debug.Log("pitch  mag " + velocity.normalized.magnitude);
//                          if (audioSource.isPlaying == false)
//                          {
//                              audioSource.clip = enemyMove.clip;
//                              audioSource.Play();
//                              //Debug.Log("clip " + audioSource.clip);

//                          }

//                      }

//                      if (enemyMove.ps)
//                      {
//                          if (enemyMove.ps.isPlaying == false)
//                          {
//                              enemyMove.ps.transform.SetParent(enemyMove.transform);
//                              enemyMove.ps.Play(true);
//                          }
//                      }
//                  }
//                  else
//                  {
//                      if (audioSource != null) audioSource.Stop();
//                      if (enemyMove.ps != null) enemyMove.ps.Stop();

//                  }




//              }
//          ).Run();

//    }

//}


