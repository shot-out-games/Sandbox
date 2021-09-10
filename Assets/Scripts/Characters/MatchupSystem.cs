using System;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;


public class MatchupSystem : SystemBase
{




    protected override void OnUpdate()
    {
        var effectsGroup = GetComponentDataFromEntity<EffectsComponent>(false);



        //get closest player to enemy
        Entities.WithAll<DeadComponent>().WithAll<EnemyComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
        (
            (
                Transform enemyTransform,//traverse enemies
                Entity enemyEntity
            ) =>
            {


                //CloseComponent added to npc only too far away (1 only because no time to figure more)
                float3 closePlayerPosition = float3.zero;
                float closePlayerMaxDistance = math.INFINITY;
                Entities.WithoutBurst().ForEach((in Entity e, in Translation translation, in CloseComponent closeComponent) =>
                    {
                        closePlayerPosition = translation.Value;
                        closePlayerMaxDistance = closeComponent.maxDistance;
                        if (closeComponent.isDamaging == false) closePlayerMaxDistance = math.INFINITY;

                    }


                    ).Run();


                Entities.WithoutBurst().WithNone<CloseComponent>().ForEach(
                    (
                        ref GunComponent gun,
                        ref RatingsComponent ratingsComponent,
                        in PlayerComponent playerComponent,
                        in Entity playerE,
                        in Translation playerTranslation
                    ) =>
                    {

                        float distance = math.distance(playerTranslation.Value, closePlayerPosition);
                        //Debug.Log("pos " + distance);
                        //gun.ChangeAmmoStats = 0;
                        //ratingsComponent.gameSpeed = ratingsComponent.speed;
                        //skip if speed power up enabled
                        if (HasComponent<Speed>(playerE))
                        {
                            if (GetComponent<Speed>(playerE).enabled) return;
                        }



                        if (distance > closePlayerMaxDistance)
                        {
                            gun.ChangeAmmoStats = distance - closePlayerMaxDistance;
                            ratingsComponent.gameSpeed = ratingsComponent.speed *
                                (100 - (distance - closePlayerMaxDistance)) / 100;
                            if (ratingsComponent.gameSpeed < 4) ratingsComponent.gameSpeed = 4;
                            //Debug.Log("rate " + gun.ChangeAmmoStats);
                            if (HasComponent<EffectsComponent>(playerE))
                            {
                                var effect = GetComponent<EffectsComponent>(playerE);
                                effect.playEffectType = EffectType.TwoClose;
                                effect.playEffectAllowed = true;
                                SetComponent<EffectsComponent>(playerE, effect);
                            }

                        }
                        else
                        {
                            if (HasComponent<EffectsComponent>(playerE))
                            {
                                var effect = GetComponent<EffectsComponent>(playerE);
                                //effect.playEffectType = EffectType.None;
                                effect.playEffectAllowed = false;
                                SetComponent<EffectsComponent>(playerE, effect);

                            }

                        }


                    }).Run();
                //end close component





                float closestDistance = math.INFINITY;
                GameObject enemy = enemyTransform.gameObject;
                GameObject player = null;
                GameObject closestPlayer = null;
                bool enemyDead = GetComponent<DeadComponent>(enemyEntity).isDead;





                Entities.WithAll<DeadComponent>().WithAll<PlayerComponent>().WithoutBurst().ForEach
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
                            float distance = math.distance(playerTransform.position, enemyTransform.position);



                            // Get a normalized vector from the guard to the player
                            var forwardVector = math.forward(enemyTransform.rotation);
                            var vectorToPlayer = playerTransform.position - enemyTransform.position;
                            var unitVecToPlayer = math.normalize(vectorToPlayer);


                            float AngleRadians = math.INFINITY;
                            float ViewDistanceSQ = math.INFINITY;
                            var dot = 1.0;
                            bool View360 = true;

                            if (HasComponent<MatchupComponent>(enemyEntity) && GetComponent<DefensiveStrategyComponent>(enemyEntity).currentRole == DefensiveRoles.None)
                            {
                                AngleRadians = GetComponent<MatchupComponent>(enemyEntity).AngleRadians;
                                ViewDistanceSQ = GetComponent<MatchupComponent>(enemyEntity).ViewDistanceSQ;
                                dot = math.dot(forwardVector, unitVecToPlayer);
                                // Use the dot product to determine if the player is within our vision cone
                                View360 = GetComponent<MatchupComponent>(enemyEntity).View360 ||
                                    GetComponent<EnemyStateComponent>(enemyEntity).MoveState == MoveStates.Chase || GetComponent<EnemyStateComponent>(enemyEntity).MoveState == MoveStates.Default;

                            }

                            //Debug.Log("rads " + math.degrees( math.abs(math.acos(dot))));
                            var canSeePlayer = (dot > 0.0f || View360) && // player is in front of us
                                math.degrees(math.abs(math.acos(dot))) < AngleRadians &&            // player is within the cone angle bounds
                                math.length(vectorToPlayer) < ViewDistanceSQ;            // player is within vision distance (we use Squared Distance to avoid sqrt calculation)



                            //float ck = math.lengthsq(vectorToPlayer);
                            //Debug.Log("ck " + ck);



                            if (distance < closestDistance && canSeePlayer)
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
                else
                {

                    if (enemy.GetComponent<EnemyWeaponAim>())
                    {
                        enemy.GetComponent<EnemyWeaponAim>().target =
                            null;
                    }
                    if (enemy.GetComponent<EnemyMelee>())
                    {
                        enemy.GetComponent<EnemyMelee>().moveUsing.target =
                            null;
                    }
                    if (enemy.GetComponent<EnemyMove>())
                    {
                        enemy.GetComponent<EnemyMove>().target =
                            null;
                    }
                }

            }
        ).Run();


        //get closest enemy to player
        Entities.WithAll<DeadComponent>().WithAll<PlayerComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
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


                Entities.WithAll<DeadComponent>().WithAll<EnemyComponent>().WithNone<SkipMatchupComponent>().WithoutBurst().ForEach
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
                        player.GetComponent<PlayerWeaponAim>().closetEnemyWeaponTargetPosition =
                            closestEnemy.GetComponent<TargetZones>().headZone.position;

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





