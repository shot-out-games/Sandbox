using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;

public class AttackerSystem : JobComponentSystem
{
    //public int counta;
    //public int countb;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
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









        Entities.WithoutBurst().ForEach(
        //(HealthBar healthBar, DynamicBuffer<CollisionComponent> collisionComponent,
        (
            Animator animator,
            HealthBar healthBar,
            in CollisionComponent collisionComponent,
            in Entity entity


        ) =>
        {
            bool dead = false;
            if (EntityManager.HasComponent<DeadComponent>(entity))
            {
                dead = EntityManager.GetComponentData<DeadComponent>(entity).isDead;
            }
            if (dead) return;



            int type_a = collisionComponent.Part_entity;
            int type_b = collisionComponent.Part_other_entity;
            Entity collision_entity_a = collisionComponent.Character_entity;
            Entity collision_entity_b = collisionComponent.Character_other_entity;
            bool isMelee = collisionComponent.isMelee;

            Entity entityA = collision_entity_a;
            Entity entityB = collision_entity_b;
            //int typeA = type_a;
            //int typeB = type_b;
            //Debug.Log("as0 " + entityA);
            //Debug.Log("as1 " + entityB);

            bool playerA = EntityManager.HasComponent(collision_entity_a, typeof(PlayerComponent));
            bool playerB = EntityManager.HasComponent(collision_entity_b, typeof(PlayerComponent));
            bool enemyA = EntityManager.HasComponent(collision_entity_a, typeof(EnemyComponent));
            bool enemyB = EntityManager.HasComponent(collision_entity_b, typeof(EnemyComponent));

            bool check = false;

            float hw = animator.GetFloat("HitWeight");


            if (playerA && enemyB || playerB && enemyA) check = true;

            if (check == true)
            {


                //Debug.Log("hp check");


                var trigger_a = EntityManager.GetComponentData<CheckedComponent>(entityA);

                bool triggerChecked_a = trigger_a.collisionChecked;
                float hitPower = 1;
                float WeaponPower = 1;

                if (triggerChecked_a == false)
                {

                    bool hasRatings = EntityManager.HasComponent<RatingsComponent>(entityA);
                    if (hasRatings && isMelee)
                    {
                        WeaponPower = EntityManager.GetComponentData<RatingsComponent>(entityA).gameWeaponPower;//should eventually check to see if weapon attached 
                                                                                                                //Debug.Log("isMelee " + WeaponPower);
                    }

                    var healthB = EntityManager.GetComponentData<HealthComponent>(entityB);
                    bool alwaysDamage = healthB.AlwaysDamage;

                    if (alwaysDamage == false)
                    {

                        bool hasMelee = EntityManager.HasComponent<MeleeComponent>(entityA);
                        if (hasMelee)
                        {
                            hitPower = EntityManager.GetComponentData<MeleeComponent>(entityA)
                                .gameHitPower * WeaponPower;

                            bool anyTouchDamage = EntityManager.GetComponentData<MeleeComponent>(entityA)
                                .anyTouchDamage;

                            if (anyTouchDamage == true && hw < .19)
                            {
                                hw = .19f;
                            }

                        }

                    }
                    else
                    {
                        hw = 1;
                        hitPower = 10;//add to arcadeGame Component later
                                      //Debug.Log("hp 1d");
                    }

                    //Debug.Log("hp " + hitPower + " hw " + hw + " wp " + WeaponPower + " game hp " + EntityManager.GetComponentData<MeleeComponent>(entityA).gameHitPower);

                    float damage = hitPower * hw;
                    //Debug.Log("damage " + damage);

                    bool inFling = false;
                    if (EntityManager.HasComponent<FlingMechanicComponent>(entityB))
                    {
                        inFling = EntityManager.GetComponentData<FlingMechanicComponent>(entityB).inFling;//flinging so causes damage not receive

                    }
                    if (inFling)
                    {

                        ecb.AddComponent<DamageComponent>(entityA,
                        new DamageComponent { DamageLanded = 0, DamageReceived = damage });
                    }
                    else
                    {

                        ecb.AddComponent<DamageComponent>(entityB,
                            new DamageComponent { DamageLanded = 0, DamageReceived = damage });

                    }
                    //for NDE
                    //var health = EntityManager.GetComponentData<HealthComponent>(entityA);
                    //health.TotalDamageReceived = health.TotalDamageReceived - 9f;
                    //if (health.TotalDamageReceived < 5) health.TotalDamageReceived = 9f;
                    //EntityManager.SetComponentData(entityA, health);

                    //for LevelUp
                    //HARA KIRI turn off skill boost when hitting Leader
                    bool skillBoost = true;
                    if (EntityManager.HasComponent(entityB, typeof(MatchupComponent)))
                    {
                        skillBoost = !EntityManager.GetComponentData<MatchupComponent>(entityB).leader;
                    }

                    bool isDead = EntityManager.GetComponentData<DeadComponent>(entityB).isDead;

                    if (isDead == false)
                    {

                        var health = EntityManager.GetComponentData<HealthComponent>(entityA);
                        var skillComponent = EntityManager.GetComponentData<SkillTreeComponent>(entityA);
                        float oppHealthFactor =
                                50f / EntityManager.GetComponentData<RatingsComponent>(entityB).maxHealth;
                        health.TotalDamageLanded = health.TotalDamageLanded + damage;
                        skillComponent.CurrentLevelXp += damage / 10f * oppHealthFactor;
                        //skillComponent.CurrentLevel = 8;
                        EntityManager.SetComponentData(entityA, health);
                        if (skillBoost)
                        {
                            EntityManager.SetComponentData(entityA, skillComponent);
                        }
                    }
                    //Debug.Log("xp " + skillComponent.CurrentLevelXp);

                    trigger_a.collisionChecked = true;
                    ecb.SetComponent<CheckedComponent>(entityA, trigger_a);


                }
            }
            //}





            if (type_b == (int)TriggerType.Ammo) //b is ammo so causes damage to entity
            {
                //damage landed not being credited to shooter currently
                int shooterTag = -1;
                Entity shooter = Entity.Null;
                bool damageCaused = false;
                if (EntityManager.HasComponent<AmmoComponent>(collision_entity_b)
                    ) //ammo entity not character if true
                {
                    shooter = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b)
                            .OwnerAmmoEntity;
                }


                //Debug.Log("ta " + type_a + " tb " + type_b);
                //Debug.Log("ea " + collision_entity_a + " eb " + collision_entity_b);
                Debug.Log("shooter " + shooter);

                if (shooter != Entity.Null)
                {
                    //Debug.Log("shooter");
                    bool isEnemy = (EntityManager.HasComponent(shooter, typeof(EnemyComponent)));

                    float damage = EntityManager.GetComponentData<GunComponent>(shooter).gameDamage;
                    //   if (shooter != collision_entity_b)
                    //{
                    //Debug.Log("shooter0");
                    AmmoComponent ammo =
                        EntityManager.GetComponentData<AmmoComponent>(collision_entity_b);

                    ammo.AmmoDead = true;
                    if (ammo.DamageCausedPreviously == true)
                    {
                        damage = 0;
                    }

                    ammo.DamageCausedPreviously = true;
                    ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);


                    ecb.AddComponent<DamageComponent>(collision_entity_a,
                            new DamageComponent
                            { DamageLanded = 0, DamageReceived = damage, StunLanded = damage });


                    ////for NDE
                    //var health = EntityManager.GetComponentData<HealthComponent>(shooter);
                    //health.TotalDamageReceived = health.TotalDamageReceived - 3f;
                    //if (health.TotalDamageReceived < 3) health.TotalDamageReceived = 3;
                    //EntityManager.SetComponentData(shooter, health);



                    //}

                }
            }


            ecb.RemoveComponent<CollisionComponent>(entity);




        }
        ).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();


        return default;
    }

}

