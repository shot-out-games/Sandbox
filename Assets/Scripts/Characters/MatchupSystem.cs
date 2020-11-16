using System;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;





public class MatchupSystem :  SystemBase
{




    protected override void OnUpdate()
    {



        Entities.WithAll<EnemyComponent>().WithNone<SkipMatchupComponent, CubeComponent>().WithoutBurst().ForEach
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








        Entities.WithAll<PlayerComponent>().WithNone<SkipMatchupComponent, CubeComponent>().WithoutBurst().ForEach
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


                Entities.WithAll<EnemyComponent>().WithNone<SkipMatchupComponent, CubeComponent>().WithoutBurst().ForEach
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


    }





}





