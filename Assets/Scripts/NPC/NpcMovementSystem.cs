using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using Unity.Jobs;





public class NpcMovementSystem : SystemBase
{

    protected override void OnUpdate()
    {

        var entityManager = World.EntityManager;


        Entities.WithoutBurst().ForEach
        (
            (
                ref NpcStateComponent npcState,
                ref NpcMovementComponent npcMovementComponent,
                in LevelCompleteComponent levelComplete,
                in Entity entity,
                in NonPlayerCharacter npcMove,
                in Animator animator,//represents other character
                in NpcRatings NpcRatings,
                in NavMeshAgent agent

            ) =>
            {

                //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;
                if (levelComplete.active == false)
                {
                    //npcMove.GetComponent<Rigidbody>().isKinematic = true;
                    agent.enabled = false;
                    return;
                }




                if (npcMove.target != null)
                {
                    float backupZoneFar = npcMovementComponent.combatStrikeDistanceZoneEnd;
                    float backupZoneClose = npcMovementComponent.combatStrikeDistanceZoneBegin;
                    //Debug.Log("chase " + npcMovementComponent.chaseOnly);
                    if (npcMovementComponent.chaseOnly)
                    {
                        backupZoneClose = 0f;
                        backupZoneFar = .2f;
                    }

                    npcMovementComponent.speedMultiple = 1;
                    MoveStates MoveState = npcState.MoveState;
                    NpcRoles npcRole = npcMove.npcRole;
                    Vector3 characterPosition = animator.transform.position;
                    Vector3 npcPosition = npcMove.target.position;
                    float dist = Vector3.Distance(npcPosition, characterPosition);

                    if (dist < backupZoneClose)//just make backupZoneClose -1 to avoid NPC doing this when you want normal follow
                    {
                        npcMovementComponent.backup = true;//only time to turn on 
                        npcMovementComponent.speedMultiple = 1.0f - dist / backupZoneClose;
                    }


                    if (npcMovementComponent.backup && dist > backupZoneFar)
                    {
                        MoveState = MoveStates.Default;
                        npcMovementComponent.backup = false;//only time to turn off
                    }
                    else if (dist >= backupZoneClose && dist <= backupZoneFar)
                    {
                        MoveState = MoveStates.Idle;
                        float speedMultiple = 0;
                        npcMovementComponent.speedMultiple = speedMultiple;

                    }
                    else if (npcMovementComponent.backup)
                    {
                        MoveState = MoveStates.Default;
                        animator.SetInteger("Zone", 2);
                        npcMove.SetDestination();
                    }
                    else if (dist < npcMove.chaseRange)
                    {
                        MoveState = MoveStates.Chase;
                        animator.SetInteger("Zone", 1);
                        npcMove.SetDestination();
                    }
                    else if (dist >= npcMove.chaseRange && npcRole == NpcRoles.Chase)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Idle;
                        npcMove.AnimationMovement();//dont chase just idle animation - sets animation param Z=0
                    }
                    else if (dist >= npcMove.chaseRange && npcRole == NpcRoles.Patrol)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Patrol;
                        npcMove.Patrol();
                    }

                    npcState = new NpcStateComponent() { MoveState = MoveState };


                    npcMove.FacePlayer();
                }

            }
        ).Run();





        Entities.WithAny<PlayerComponent>().WithoutBurst().WithStructuralChanges().WithAll<NonPlayerCharacter>().ForEach(
            (InputController inputController) =>
            {

                //bool send = inputController.leftBumperPressed || inputController.rightBumperPressed;
                bool send = inputController.rightBumperPressed;


                Entities.WithoutBurst().WithStructuralChanges().ForEach(
                    (ref NpcMovementComponent npcMovementComponent) =>
                    {
                        //npcMovementComponent.chaseOnly = send;
                        if (send == true)
                        {
                            npcMovementComponent.chaseOnly = !npcMovementComponent.chaseOnly;
                        }
                    }
                ).Run();





            }
        ).Run();







        //return default;
    }

}



public class NpcMovementGenericSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        var entityManager = World.EntityManager;


        Entities.WithoutBurst().ForEach
        (
            (
                ref NpcStateComponent npcState,
                ref NpcMovementComponent npcMovementComponent,
                in LevelCompleteComponent levelComplete,
                in Entity entity,
                in NonPlayerCharacterGeneric npcMove,
                in Animator animator,//represents other character
                in NpcRatings NpcRatings,
                in NavMeshAgent agent

            ) =>
            {

                //if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;
                if (levelComplete.active == false)
                {
                    //npcMove.GetComponent<Rigidbody>().isKinematic = true;
                    agent.enabled = false;
                    return;
                }




                if (npcMove.target != null)
                {
                    float backupZoneFar = npcMovementComponent.combatStrikeDistanceZoneEnd;
                    float backupZoneClose = npcMovementComponent.combatStrikeDistanceZoneBegin;
                    //Debug.Log("chase " + npcMovementComponent.chaseOnly);
                    if (npcMovementComponent.chaseOnly)
                    {
                        backupZoneClose = 0f;
                        backupZoneFar = .2f;
                    }

                    npcMovementComponent.speedMultiple = 1;
                    MoveStates MoveState = npcState.MoveState;
                    NpcRoles npcRole = npcMove.npcRole;
                    Vector3 characterPosition = animator.transform.position;
                    Vector3 npcPosition = npcMove.target.position;
                    float dist = Vector3.Distance(npcPosition, characterPosition);

                    if (dist < backupZoneClose)//just make backupZoneClose -1 to avoid NPC doing this when you want normal follow
                    {
                        npcMovementComponent.backup = true;//only time to turn on 
                        npcMovementComponent.speedMultiple = 1.0f - dist / backupZoneClose;
                    }


                    if (npcMovementComponent.backup && dist > backupZoneFar)
                    {
                        MoveState = MoveStates.Default;
                        npcMovementComponent.backup = false;//only time to turn off
                    }
                    else if (dist >= backupZoneClose && dist <= backupZoneFar)
                    {
                        MoveState = MoveStates.Idle;
                        float speedMultiple = 0;
                        npcMovementComponent.speedMultiple = speedMultiple;

                    }
                    else if (npcMovementComponent.backup)
                    {
                        MoveState = MoveStates.Default;
                        //animator.SetInteger("Zone", 2);
                        npcMove.SetDestination();
                    }
                    else if (dist < npcMove.chaseRange)
                    {
                        MoveState = MoveStates.Chase;
                        animator.SetInteger("Zone", 1);
                        npcMove.SetDestination();
                    }
                    else if (dist >= npcMove.chaseRange && npcRole == NpcRoles.Chase)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Idle;
                        npcMove.AnimationMovement();//dont chase just idle animation - sets animation param Z=0
                    }
                    else if (dist >= npcMove.chaseRange && npcRole == NpcRoles.Patrol)
                    {
                        animator.SetInteger("Zone", 1);
                        MoveState = MoveStates.Patrol;
                        npcMove.Patrol();
                    }

                    npcState = new NpcStateComponent() { MoveState = MoveState };


                    npcMove.FacePlayer();
                }

            }
        ).Run();





        Entities.WithAny<PlayerComponent>().WithoutBurst().WithStructuralChanges().ForEach(
            (InputController inputController) =>
            {

                //bool send = inputController.leftBumperPressed || inputController.rightBumperPressed;
                bool send = inputController.rightBumperPressed;


                Entities.WithoutBurst().WithStructuralChanges().WithAll<NonPlayerCharacterGeneric>().ForEach(
                    (ref NpcMovementComponent npcMovementComponent) =>
                    {
                        //npcMovementComponent.chaseOnly = send;
                        if (send == true)
                        {
                            npcMovementComponent.chaseOnly = !npcMovementComponent.chaseOnly;
                        }
                    }
                ).Run();





            }
        ).Run();







        return default;
    }

}



