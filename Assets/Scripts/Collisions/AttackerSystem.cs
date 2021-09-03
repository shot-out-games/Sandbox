using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
[UpdateBefore(typeof(CollisionSystem))]



public class AttackerSystem : SystemBase
{
    //public int counta;
    //public int countb;

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);



        Entities.WithoutBurst().ForEach(
            //(HealthBar healthBar, DynamicBuffer<CollisionComponent> collisionComponent,
            (
                ref CheckedComponent checkedComponent


            ) =>
            {
                int countDown = 60;
                if (checkedComponent.collisionChecked == true && checkedComponent.timer < countDown)
                {
                    checkedComponent.timer++;
                }
                else
                {
                    checkedComponent.collisionChecked = false;
                    checkedComponent.timer = 0;
                }



            }
            ).Run();









        #region MyRegion

        //        Entities.WithoutBurst().ForEach(
        //        //(HealthBar healthBar, DynamicBuffer<CollisionComponent> collisionComponent,
        //        (
        //            Animator animator,
        //            HealthBar healthBar,
        //            in DeadComponent dead,
        //            in CollisionComponent collisionComponent,
        //            in Entity entity


        //        ) =>
        //        {
        //            if (dead.isDead == true) return;



        //            int type_a = collisionComponent.Part_entity;
        //            int type_b = collisionComponent.Part_other_entity;
        //            Entity collision_entity_a = collisionComponent.Character_entity;
        //            Entity collision_entity_b = collisionComponent.Character_other_entity;
        //            bool isMelee = collisionComponent.isMelee;

        //            Entity entityA = collision_entity_a;
        //            Entity entityB = collision_entity_b;

        //            bool playerA = EntityManager.HasComponent(collision_entity_a, typeof(PlayerComponent));
        //            bool playerB = EntityManager.HasComponent(collision_entity_b, typeof(PlayerComponent));
        //            bool enemyA = EntityManager.HasComponent(collision_entity_a, typeof(EnemyComponent));
        //            bool enemyB = EntityManager.HasComponent(collision_entity_b, typeof(EnemyComponent));

        //            bool check = false;

        //            float hw = animator.GetFloat("HitWeight");


        //            if (playerA && enemyB || playerB && enemyA) check = true;

        //            if (check == true)
        //            {

        //                var trigger_a = EntityManager.GetComponentData<CheckedComponent>(entityA);

        //                bool triggerChecked_a = trigger_a.collisionChecked;
        //                float hitPower = 1;
        //                float WeaponPower = 1;

        //                if (triggerChecked_a == false)
        //                {

        //                    bool hasRatings = EntityManager.HasComponent<RatingsComponent>(entityA);
        //                    if (hasRatings && isMelee)
        //                    {
        //                        WeaponPower = EntityManager.GetComponentData<RatingsComponent>(entityA).gameWeaponPower;//should eventually check to see if weapon attached 
        //                                                                                                                //Debug.Log("isMelee " + WeaponPower);
        //                    }

        //                    var healthB = EntityManager.GetComponentData<HealthComponent>(entityB);
        //                    bool alwaysDamage = healthB.AlwaysDamage;

        //                    if (alwaysDamage == false)
        //                    {

        //                        bool hasMelee = EntityManager.HasComponent<MeleeComponent>(entityA);
        //                        if (hasMelee)
        //                        {
        //                            hitPower = EntityManager.GetComponentData<MeleeComponent>(entityA)
        //                                .gameHitPower * WeaponPower;

        //                            bool anyTouchDamage = EntityManager.GetComponentData<MeleeComponent>(entityA)
        //                                .anyTouchDamage;

        //                            if (anyTouchDamage == true && hw < .19)
        //                            {
        //                                hw = .19f;
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        hw = 1;
        //                        hitPower = 10;//add to arcadeGame Component later
        //                    }


        //                    float damage = hitPower * hw;
        //                    Debug.Log("damage " + damage);

        //                    bool inFling = false;
        //                    if (EntityManager.HasComponent<FlingMechanicComponent>(entityB))
        //                    {
        //                        inFling = EntityManager.GetComponentData<FlingMechanicComponent>(entityB).inFling;//flinging so causes damage not receive

        //                    }


        //                    if (inFling)
        //                    {
        //                        if (HasComponent<ScoreComponent>(entityB))
        //                        {
        //                            var scoreComponent = EntityManager.GetComponentData<ScoreComponent>(entityB);
        //                            scoreComponent.pointsScored = true;
        //                            scoreComponent.scoredAgainstEntity = entityA;
        //                            EntityManager.SetComponentData(entityB, scoreComponent);
        //                        }



        //                        ecb.AddComponent<DamageComponent>(entityA,
        //                        new DamageComponent { DamageLanded = 0, DamageReceived = damage });
        //                    }
        //                    else
        //                    {

        //                        if (HasComponent<ScoreComponent>(entityA))
        //                        {
        //                            var scoreComponent = EntityManager.GetComponentData<ScoreComponent>(entityA);
        //                            scoreComponent.pointsScored = true;
        //                            scoreComponent.scoredAgainstEntity = entityB;
        //                            EntityManager.SetComponentData(entityA, scoreComponent);
        //                        }



        //                        ecb.AddComponent<DamageComponent>(entityB,
        //                            new DamageComponent { DamageLanded = 0, DamageReceived = damage });

        //                    }
        //                    //for NDE
        //                    //var health = EntityManager.GetComponentData<HealthComponent>(entityA);
        //                    //health.TotalDamageReceived = health.TotalDamageReceived - 9f;
        //                    //if (health.TotalDamageReceived < 5) health.TotalDamageReceived = 9f;
        //                    //EntityManager.SetComponentData(entityA, health);

        //                    //for LevelUp
        //                    //HARA KIRI turn off skill boost when hitting Leader
        //                    bool skillBoost = true;
        //                    if (EntityManager.HasComponent(entityB, typeof(MatchupComponent)))
        //                    {
        //                        skillBoost = !EntityManager.GetComponentData<MatchupComponent>(entityB).leader;
        //                    }

        //                    bool isDead = EntityManager.GetComponentData<DeadComponent>(entityB).isDead;

        //                    if (isDead == false)
        //                    {

        //                        var health = EntityManager.GetComponentData<HealthComponent>(entityA);
        //                        var skillComponent = EntityManager.GetComponentData<SkillTreeComponent>(entityA);
        //                        float oppHealthFactor =
        //                                50f / EntityManager.GetComponentData<RatingsComponent>(entityB).maxHealth;
        //                        health.TotalDamageLanded = health.TotalDamageLanded + damage;
        //                        skillComponent.CurrentLevelXp += damage / 10f * oppHealthFactor;
        //                        //skillComponent.CurrentLevel = 8;
        //                        EntityManager.SetComponentData(entityA, health);
        //                        if (skillBoost)
        //                        {
        //                            EntityManager.SetComponentData(entityA, skillComponent);
        //                        }
        //                    }
        //                    //Debug.Log("xp " + skillComponent.CurrentLevelXp);

        //                    trigger_a.collisionChecked = true;
        //                    ecb.SetComponent<CheckedComponent>(entityA, trigger_a);


        //                }
        //            }
        //            //}





        //            if (type_b == (int)TriggerType.Ammo) //b is ammo so causes damage to entity
        //            {
        //                //damage landed not being credited to shooter currently
        //                int shooterTag = -1;
        //                Entity shooter = Entity.Null;
        //                bool damageCaused = false;
        //                if (EntityManager.HasComponent<AmmoComponent>(collision_entity_b)
        //                    ) //ammo entity not character if true
        //                {
        //                    shooter = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b)
        //                            .OwnerAmmoEntity;
        //                }


        //                //Debug.Log("ta " + type_a + " tb " + type_b);
        //                Debug.Log("ea " + collision_entity_a + " eb " + collision_entity_b);
        //                Debug.Log("shooter " + shooter);

        //                if (shooter != Entity.Null)
        //                {
        //                    //Debug.Log("shooter");
        //                    bool isEnemy = (EntityManager.HasComponent(shooter, typeof(EnemyComponent)));

        //                    float damage = EntityManager.GetComponentData<GunComponent>(shooter).gameDamage;
        //                    //   if (shooter != collision_entity_b)
        //                    //{
        //                    //Debug.Log("shooter0");
        //                    AmmoComponent ammo =
        //                        EntityManager.GetComponentData<AmmoComponent>(collision_entity_b);

        //                    ammo.AmmoDead = true;
        //                    if (ammo.DamageCausedPreviously == true)
        //                    {
        //                        damage = 0;
        //                    }

        //                    ammo.DamageCausedPreviously = true;
        //                    ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);


        //                    ecb.AddComponent<DamageComponent>(collision_entity_a,
        //                            new DamageComponent
        //                            { DamageLanded = 0, DamageReceived = damage, StunLanded = damage });


        //                    ////for NDE
        //                    //var health = EntityManager.GetComponentData<HealthComponent>(shooter);
        //                    //health.TotalDamageReceived = health.TotalDamageReceived - 3f;
        //                    //if (health.TotalDamageReceived < 3) health.TotalDamageReceived = 3;
        //                    //EntityManager.SetComponentData(shooter, health);



        //                    //}

        //                }
        //            }


        //            ecb.RemoveComponent<CollisionComponent>(entity);




        //        }
        //        ).Run();


        //        ecb.Playback(EntityManager);
        //        ecb.Dispose();


        //        //return default;
        //    }

        //}

        #endregion




        Entities.WithoutBurst().ForEach(
        //(HealthBar healthBar, DynamicBuffer<CollisionComponent> collisionComponent,
        (
            Animator animator,
            HealthBar healthBar,
            in DeadComponent dead,
            in CollisionComponent collisionComponent,
            in Entity entity


        ) =>
        {
            if (dead.isDead == true) return;



            int type_a = collisionComponent.Part_entity;
            int type_b = collisionComponent.Part_other_entity;
            Entity collision_entity_a = collisionComponent.Character_entity;
            Entity collision_entity_b = collisionComponent.Character_other_entity;
            bool isMelee = collisionComponent.isMelee;

            Entity entityA = collision_entity_a;
            Entity entityB = collision_entity_b;

            bool playerA = HasComponent<PlayerComponent>(collision_entity_a);
            bool playerB = HasComponent<PlayerComponent>(collision_entity_b);
            bool enemyA = HasComponent<EnemyComponent>(collision_entity_a);
            bool enemyB = HasComponent<EnemyComponent>(collision_entity_b);

            bool check = false;

            float hw = animator.GetFloat("HitWeight");
            if (playerA && enemyB || playerB && enemyA) check = true;

            if (check == true)
            {

                var trigger_a = GetComponent<CheckedComponent>(entityA);
                if (trigger_a.collisionChecked == false)
                {

                    float hitPower = 10;//need to be able to change eventually
                    if (HasComponent<RatingsComponent>(entityA))
                    {
                        hitPower = GetComponent<RatingsComponent>(entityA).hitPower;
                        //Debug.Log("hit power " + hitPower);
                    }
                    if (HasComponent<HealthComponent>(entityA))
                    {
                        bool alwaysDamage = GetComponent<HealthComponent>(entityA).AlwaysDamage;
                        if (alwaysDamage) hw = 1;//
                        //Debug.Log("hit power " + hitPower);
                    }

                    //float WeaponPower = 1;
                    //hw = 1;
                    float damage = hitPower * hw;

                    ecb.AddComponent<DamageComponent>(entityA,
                        new DamageComponent { DamageLanded = damage, DamageReceived = 0 });


                    ecb.AddComponent<DamageComponent>(entityB,
                        new DamageComponent { DamageLanded = 0, DamageReceived = damage });

                    if (HasComponent<SkillTreeComponent>(entityA))
                    {
                        var skill = GetComponent<SkillTreeComponent>(entityA);
                        skill.CurrentLevelXp += damage;
                        //Debug.Log("xp " + skill.CurrentLevelXp);
                        SetComponent<SkillTreeComponent>(entityA, skill);
                    }


                    if (HasComponent<ScoreComponent>(entityA) && damage != 0)
                    {

                        var scoreComponent = GetComponent<ScoreComponent>(entityA);
                        scoreComponent.pointsScored = true;
                        scoreComponent.scoredAgainstEntity = entityB;
                        //add specific score component stuff to melee
                        SetComponent(entityA, scoreComponent);
                    }










                    trigger_a.collisionChecked = true;



                    ecb.SetComponent<CheckedComponent>(entityA, trigger_a);
                }


            }



            //Debug.Log("ty b " + type_b + " ty a " + type_a);

            if (type_b == (int)TriggerType.Ammo && HasComponent<TriggerComponent>(collision_entity_a)
                                                && HasComponent<TriggerComponent>(collision_entity_b)) //b is ammo so causes damage to entity
            {
                Entity shooter = Entity.Null;
                //if (EntityManager.HasComponent<AmmoComponent>(collision_entity_b)) //ammo entity not character if true
                //{
                shooter = GetComponent<TriggerComponent>(collision_entity_b)
                    .ParentEntity;
                //}
                //Debug.Log("ta " + type_a + " tb " + type_b);
                //Debug.Log("ea " + collision_entity_a + " eb " + collision_entity_b);
                Debug.Log("shooter " + shooter);

                if (shooter != Entity.Null && HasComponent<AmmoComponent>(collision_entity_b))
                {
                    bool isEnemyShooter = HasComponent<EnemyComponent>(shooter);
                    Entity target = GetComponent<TriggerComponent>(collision_entity_a)
                        .ParentEntity;
                    bool isEnemyTarget = HasComponent<EnemyComponent>(target);
                    Debug.Log("sh  " + shooter);
                    Debug.Log("cea " + collision_entity_a);
                    Debug.Log("ceb " + collision_entity_b);
                    AmmoComponent ammo =
                        GetComponent<AmmoComponent>(collision_entity_b);
                    AmmoDataComponent ammoData =
                        GetComponent<AmmoDataComponent>(collision_entity_b);

                    float damage = GetComponent<GunComponent>(shooter).gameDamage;
                    //Debug.Log("damage " + damage);
                    ammo.AmmoDead = true;


                    if (ammo.DamageCausedPreviously && ammo.frameSkipCounter > ammo.framesToSkip)//count in ammosystem
                    {
                        ammo.DamageCausedPreviously = false;
                        ammo.frameSkipCounter = 0;
                    }

                    //bool skip = ammo.frameSkipCounter < ammo.framesToSkip && ammo.frameSkipCounter >= 1;
                    //if (skip) ammo.frameSkipCounter = ammo.frameSkipCounter + 1;



                    if (ammo.DamageCausedPreviously || ammoData.ChargeRequired == true && ammo.Charged == false  ||
                        isEnemyShooter == isEnemyTarget
                        )
                    {
                        
                        damage = 0;
                    }

                    if (HasComponent<DeadComponent>(collision_entity_a) == false ||
                        GetComponent<DeadComponent>(collision_entity_a).isDying)
                    {
                        damage = 0;
                    }


                    ammo.DamageCausedPreviously = true;

                    ecb.AddComponent<DamageComponent>(shooter,
                            new DamageComponent
                            { DamageLanded = damage, DamageReceived = 0});


                    ecb.AddComponent<DamageComponent>(collision_entity_a,
                            new DamageComponent
                            { DamageLanded = 0, DamageReceived = damage, StunLanded = damage });

                    if (HasComponent<SkillTreeComponent>(shooter))
                    {
                        var skill = GetComponent<SkillTreeComponent>(shooter);
                        skill.CurrentLevelXp += damage;
                        //Debug.Log("xp " + skill.CurrentLevelXp);
                        SetComponent<SkillTreeComponent>(shooter, skill);
                    }



                    if (HasComponent<ScoreComponent>(shooter) && damage != 0)
                    {

                        var scoreComponent = GetComponent<ScoreComponent>(shooter);
                        scoreComponent.addBonus = 0;
                        //for gmtk bonus for charged (blocked)
                        if (ammo.Charged == true && isEnemyShooter == false && isEnemyTarget == true)
                        {
                            scoreComponent.addBonus = scoreComponent.defaultPointsScored * 1;
                            ammo.Charged = false;

                        }

                        scoreComponent.scoringAmmoEntity = ammo.ammoEntity;
                        scoreComponent.pointsScored = true;
                        scoreComponent.scoredAgainstEntity = collision_entity_a;
                        SetComponent(shooter, scoreComponent);
                    }

                    ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);


                }
            }







            ecb.RemoveComponent<CollisionComponent>(entity);




        }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();


        //return default;
    }

}

