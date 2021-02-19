using System;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;


public class MatchupSystem :  SystemBase
{




    protected override void OnUpdate()
    {



        //get closest player to enemy
        Entities.WithAll<EnemyComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
        (
            (
                Transform enemyTransform,//traverse enemies
                Entity enemyEntity
            ) =>
            {

                float closestDistance = math.INFINITY;
                GameObject enemy = enemyTransform.gameObject;
                GameObject player = null;
                GameObject closestPlayer = null;
                bool enemyDead = GetComponent<DeadComponent>(enemyEntity).isDead;


                //CloseComponent added to npc only too far away (1 only because no time to figure more)
                float3 closePlayerPosition = float3.zero;
                float closePlayerMaxDistance = math.INFINITY;
                Entities.WithoutBurst().ForEach((Entity e, Translation translation, CloseComponent closeComponent) =>
                    {
                        closePlayerPosition = translation.Value;
                        closePlayerMaxDistance = closeComponent.maxDistance;

                    }


                    ).Run();


                Entities.WithoutBurst().WithNone<CloseComponent>().ForEach(
                    (PlayerComponent playerComponent, Entity playerE, Translation playerTranslation, ref GunComponent gun) =>
                    {

                        float distance = math.distance(playerTranslation.Value, closePlayerPosition);
                        //Debug.Log("pos " + distance);
                        gun.ChangeAmmoStats = 0;
                        if (distance > closePlayerMaxDistance)
                        {
                            gun.ChangeAmmoStats = distance;
                            Debug.Log("change write");

                        }


                    }).Run();







                Entities.WithAll<PlayerComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
                (
                    (
                        Transform playerTransform,//find closest player
                        Entity playerEntity
                    ) =>
                    {
                        bool playerDead = GetComponent<DeadComponent>(playerEntity).isDead;
                        player = playerTransform.gameObject;

                        if (playerDead == false && enemyDead == false)
                        {
                            float distance = Vector3.Distance(playerTransform.position, enemyTransform.position);
                            if (distance < closestDistance)
                            {
                                closestPlayer = player;
                                closestDistance = distance;
                            }
                        }

                    }
                ).Run();


                if (closestPlayer != null)
                {
                    if (enemy.GetComponent<EnemyWeaponAim>())
                    {
                        enemy.GetComponent<EnemyWeaponAim>().target =
                            closestPlayer.GetComponent<TargetZones>().headZone;
                    }
                    if (enemy.GetComponent<EnemyMelee>())
                    {
                        enemy.GetComponent<EnemyMelee>().moveUsing.target =
                            closestPlayer.GetComponent<TargetZones>().headZone;
                    }
                    if (enemy.GetComponent<EnemyMove>())
                    {
                        enemy.GetComponent<EnemyMove>().target =
                            closestPlayer.transform;
                    }

                }

            }
        ).Run();


        //get closest enemy to player
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
                bool playerDead = GetComponent<DeadComponent>(playerEntity).isDead;


                Entities.WithAll<EnemyComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
                (
                    (
                        Transform enemyTransform,//find closest enemy
                        Entity enemyEntity
                    ) =>
                    {
                        bool enemyDead = GetComponent<DeadComponent>(enemyEntity).isDead;
                        enemy = enemyTransform.gameObject;

                        if (playerDead == false && enemyDead == false)
                        {
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
                    if (player.GetComponent<PlayerCombat>())
                    {
                        player.GetComponent<PlayerCombat>().moveUsing.target =
                            closestEnemy.GetComponent<TargetZones>().headZone;
                    }

                }

            }
        ).Run();

        //-----------------------------------------leader override--------------------------------------------

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





