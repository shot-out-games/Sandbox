using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;

public class AttackerSystem : JobComponentSystem
{
    public int counta;
    public int countb;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


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

                    Entity entityA = collision_entity_a;
                    Entity entityB = collision_entity_b;
                    //int typeA = type_a;
                    //int typeB = type_b;


                    bool playerA = EntityManager.HasComponent(collision_entity_a, typeof(PlayerComponent));
                    bool playerB = EntityManager.HasComponent(collision_entity_b, typeof(PlayerComponent));
                    bool enemyA = EntityManager.HasComponent(collision_entity_a, typeof(EnemyComponent));
                    bool enemyB = EntityManager.HasComponent(collision_entity_b, typeof(EnemyComponent));

                    bool check = false;

                    float hw = animator.GetFloat("HitWeight");

                    Debug.Log("t b " + type_b + " t a " + type_a);


                    if (playerA && enemyB || playerB && enemyA) check = true;

                    if (check == true)
                    {

                        var trigger_a = EntityManager.GetComponentData<CheckedComponent>(entityA);

                        bool triggerChecked_a = trigger_a.collisionChecked;
                        float hitPower = 1;



                        //bool enemy = EntityManager.HasComponent(entityA, typeof(EnemyComponent));

                        //if (enemy == false)
                        //{
                        //Debug.Log("t b " + type_b + " t a " + type_a);


                        if (triggerChecked_a == false)
                        {

                            bool hasMelee = EntityManager.HasComponent<MeleeComponent>(entityA);
                            if (hasMelee)
                            {
                                hitPower = EntityManager.GetComponentData<MeleeComponent>(entityA)
                                    .gameHitPower;

                                bool anyTouchDamage = EntityManager.GetComponentData<MeleeComponent>(entityA)
                                    .anyTouchDamage;




                                if (anyTouchDamage == true && hw < .19)
                                {
                                    hw = .19f;
                                    Debug.Log(hitPower);
                                }


                            }


                            float damage = hitPower * hw;
                            Debug.Log("damage " + damage);



                            ecb.AddComponent<DamageComponent>(entityB,
                                new DamageComponent { DamageLanded = 0, DamageReceived = damage });


                            //for NDE
                            var health = EntityManager.GetComponentData<HealthComponent>(entityA);
                            health.TotalDamageReceived = health.TotalDamageReceived - 9f;
                            if (health.TotalDamageReceived < 5) health.TotalDamageReceived = 9f;
                            EntityManager.SetComponentData(entityA, health);



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

