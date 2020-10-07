using System;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;





public class MatchupSystem : JobComponentSystem
{




    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {



        Entities.WithAll<EnemyComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
        (
            (
                Transform enemyTransform,//traverse enemies
                Entity enemyEntity
            ) =>
            {

                float closestDistance = Mathf.Infinity;
                GameObject enemy = enemyTransform.gameObject;
                GameObject player = null;
                GameObject closestPlayer = null;
                bool enemyDead = EntityManager.GetComponentData<DeadComponent>(enemyEntity).isDead;

                Entities.WithAny<PlayerComponent, NpcComponent>().WithoutBurst().ForEach
                (
                    (
                        Transform playerTransform,//find closest player
                        Entity playerEntity
                    ) =>
                    {
                        bool playerDead = EntityManager.GetComponentData<DeadComponent>(playerEntity).isDead;
                        player = playerTransform.gameObject;

                        if (playerDead == false && enemyDead == false)
                        {
                            bool skipMatchup = EntityManager.HasComponent<SkipMatchupComponent>(playerEntity);


                            if (EntityManager.HasComponent<NpcComponent>(playerEntity) == true)
                            {
                                if (EntityManager.GetComponentData<NpcComponent>(playerEntity).active == false)
                                    skipMatchup = true;
                                if (EntityManager.HasComponent<LevelCompleteComponent>(playerEntity) == true)
                                {
                                    if (EntityManager.GetComponentData<LevelCompleteComponent>(playerEntity).targetReached == true)
                                        skipMatchup = true;
                                }
                            }
                            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
                            if (distance < closestDistance & skipMatchup == false)
                            {
                                closestPlayer = player;
                                closestDistance = distance;
                            }
                        }



                    }
                ).Run();



                if (closestPlayer != null)
                {
                    if (enemy.GetComponent<EnemyMelee>() && closestPlayer.GetComponent<TargetZones>())
                    {
                        enemy.GetComponent<EnemyMelee>().moveUsing.target =
                            closestPlayer.GetComponent<TargetZones>().headZone;
                    }
                }

                if (closestPlayer != null)
                {
                    if (enemy.GetComponent<EnemyWeaponAim>())
                    {
                        enemy.GetComponent<EnemyWeaponAim>().target =
                            closestPlayer.GetComponent<TargetZones>().headZone;

                    }
                }

                if (closestPlayer != null)
                {
                    if (enemy.GetComponent<EnemyMove>())
                    {
                        enemy.GetComponent<EnemyMove>().target = closestPlayer.transform;
                    }
                }

            }
        ).Run();








        Entities.WithAll<PlayerComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
        (
            (
                Transform playerTransform,//traverse enemies
                Entity playerEntity
            ) =>
            {

                float closestDistance = Mathf.Infinity;
                GameObject player = playerTransform.gameObject;
                GameObject enemy = null;
                GameObject closestEnemy = null;
                bool playerDead = EntityManager.GetComponentData<DeadComponent>(playerEntity).isDead;


                Entities.WithAll<EnemyComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
                (
                    (
                        Transform enemyTransform,//find closest enemy
                        Entity enemyEntity
                    ) =>
                    {
                        bool enemyDead = EntityManager.GetComponentData<DeadComponent>(enemyEntity).isDead;
                        enemy = enemyTransform.gameObject;

                        if (playerDead == false && enemyDead == false)
                        {
                            float dir = Vector3.Dot(playerTransform.forward, enemyTransform.forward);
                            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
                            if (distance < closestDistance)
                            {
                                closestEnemy = enemy;
                                closestDistance = distance;
                            }
                        }


                    }
                ).Run();


                if (closestEnemy != null)
                {
                    if (player.GetComponent<PlayerWeaponAim>())
                    {
                        player.GetComponent<PlayerWeaponAim>().target =
                            closestEnemy.GetComponent<TargetZones>().headZone;
                    }
                }

                if (closestEnemy != null)
                {
                    //Debug.Log("closest " + closestEnemy.name);
                    if (player.GetComponent<PlayerCombat>())
                    {
                        player.GetComponent<PlayerCombat>().moveUsing.target =
                            closestEnemy.GetComponent<TargetZones>().headZone;
                    }
                }



            }
        ).Run();




        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        Entities.WithoutBurst().ForEach//npc to static object distance
        (
        (
             ref NpcMovementComponent npcMovementComponent,
             in Transform npcTransform,
             in NonPlayerCharacter npc,
             in NpcComponent npcComponent,
             in Entity npcEntity
        ) =>
        {

            float closestDistance = Mathf.Infinity;
            float closestPlayerDistance = Mathf.Infinity;
            float closestItemDistance = Mathf.Infinity;

            Transform closestTransform = null;
            Transform closestPlayerTransform = null;
            Transform closestItemTransform = null;

            bool npcDead = EntityManager.GetComponentData<DeadComponent>(npcEntity).isDead;
            bool chaseOnly = npcMovementComponent.chaseOnly;

            Vector3 npcPosition = npcTransform.position;


            Entities.WithNone<SkipMatchupComponent>().WithStructuralChanges().WithoutBurst().ForEach
            (
                (

                     DestinationEligibleComponent destination,
                     Transform objectTransform,//find closest player
                     Entity objectEntity
                ) =>
                {

                    if (npcDead == false)
                    {
                        float distance = Vector3.Distance(npcPosition, objectTransform.position);
                        if (destination.priority && chaseOnly)
                        {
                            distance = -1;
                        }
                        if (distance < closestDistance)
                        {
                            closestTransform = objectTransform;
                            closestDistance = distance;
                        }

                        bool isPlayer = EntityManager.HasComponent<PlayerComponent>(objectEntity);
                        if (isPlayer)
                        {
                            if (distance < closestPlayerDistance)
                            {
                                closestPlayerTransform = objectTransform;
                                closestPlayerDistance = distance;
                            }
                        }
                        else
                        {
                            if (distance < closestItemDistance)
                            {
                                closestItemTransform = objectTransform;
                                closestItemDistance = distance;
                            }
                        }

                    }


                }
            ).Run();

            bool npcLevelActive = EntityManager.GetComponentData<LevelCompleteComponent>(npcEntity).active;
            bool npcActive = EntityManager.GetComponentData<NpcComponent>(npcEntity).active;


            if (closestTransform != null && npcLevelActive == true && npcActive == true)
            {
         
                if (closestItemDistance < 12.0f || closestItemDistance < closestPlayerDistance)
                {
                    npcMovementComponent.chaseOnly = true;
                    npc.target = closestItemTransform;
                }
                else
                {
                    npc.target = closestTransform;
                }

            }

        }
    ).Run();





        Entities.WithoutBurst().WithStructuralChanges().WithAll<MatchupComponent>().ForEach
        (
            (Entity enemy, Transform enemyTransform) =>
            {
                MatchupComponent enemyMatchupComponent = EntityManager.GetComponentData<MatchupComponent>(enemy);

                Entities.WithoutBurst().WithStructuralChanges().WithAll<MatchupComponent>().ForEach
                (
                    (Entity player, in Transform playerTransform) =>
                    {
                        MatchupComponent playerMatchupComponent =
                            EntityManager.GetComponentData<MatchupComponent>(player);
                        if (enemyMatchupComponent.matchupClosest == false && playerMatchupComponent.leader == true)
                        {
                            if (enemyTransform.GetComponent<EnemyMelee>() && playerTransform.GetComponent<TargetZones>())
                            {
                                enemyTransform.GetComponent<EnemyMelee>().moveUsing.target =
                                playerTransform.GetComponent<TargetZones>().headZone;
                            }
                            if (enemyTransform.GetComponent<EnemyWeaponAim>())
                            {
                                enemyTransform.GetComponent<EnemyWeaponAim>().target =
                                playerTransform.GetComponent<TargetZones>().headZone;
                            }
                            if (enemyTransform.GetComponent<EnemyMove>())
                            {
                                enemyTransform.GetComponent<EnemyMove>().target = playerTransform;
                            }

                        }

                    }
                ).Run();
            }

        ).Run();




        return default;
    }





}





