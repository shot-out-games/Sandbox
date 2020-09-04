using Rewired;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;

public class AttackerSystem : JobComponentSystem
{


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

                bool isPlayer_a = EntityManager.HasComponent<PlayerComponent>(collision_entity_a);
                bool isPlayer_b = EntityManager.HasComponent<PlayerComponent>(collision_entity_b);
                bool isEnemy_a = EntityManager.HasComponent<EnemyComponent>(collision_entity_a);
                bool isEnemy_b = EntityManager.HasComponent<EnemyComponent>(collision_entity_b);

                bool hasTrigger_a = EntityManager.HasComponent<TriggerComponent>(collision_entity_a);
                bool hasTrigger_b = EntityManager.HasComponent<TriggerComponent>(collision_entity_b);
                if (hasTrigger_a == false || hasTrigger_b == false) return;


                var trigger_a = EntityManager.GetComponentData<TriggerComponent>(collision_entity_a);
                var trigger_b = EntityManager.GetComponentData<TriggerComponent>(collision_entity_b);

                bool triggerChecked_a = trigger_a.triggerChecked;
                bool triggerChecked_b = trigger_b.triggerChecked;


                //Debug.Log("player a " + isPlayer_a + " enemy b " + isEnemy_b);
                //Debug.Log("player b " + isPlayer_b + " enemy a " + isEnemy_a);
                //if (attackStarted && enemyAttackComponent.AttackStage == AttackStages.No)



                float hw = animator.GetFloat("HitWeight");
                Debug.Log("hw " + hw);

                if (isPlayer_b && isEnemy_a) //b is ammo so causes damage to entity
                {
                    if (type_b == (int)TriggerType.Chest &&
                        (type_a == (int)TriggerType.LeftHand || type_a == (int)TriggerType.RightHand)
                        && hw > .05
                        && triggerChecked_a == false
                        && triggerChecked_b == false
                        )
                    {
                        float damage = 1 * hw;

                        ecb.AddComponent<DamageComponent>(collision_entity_b,
                            new DamageComponent { DamageLanded = 0, DamageReceived = damage });

                        trigger_a.triggerChecked = true;
                        trigger_b.triggerChecked = true;
                        ecb.SetComponent<TriggerComponent>(collision_entity_a, trigger_a);
                        ecb.SetComponent<TriggerComponent>(collision_entity_a, trigger_b);

                        //ecb.AddComponent<DamageComponent>(collision_entity_a,
                        // new DamageComponent { DamageLanded = hw * 10, DamageReceived = 0 });

                        Debug.Log("player b " + damage);

                    }
                }
                else if (isPlayer_a && isEnemy_b) //b is ammo so causes damage to entity
                {
                    if (type_a == (int)TriggerType.Chest &&
                        (type_b == (int)TriggerType.LeftHand || type_b == (int)TriggerType.RightHand)
                        && hw > .05
                        && triggerChecked_a == false
                        && triggerChecked_b == false
                        )
                    {

                        float damage = 1 * hw;

                        ecb.AddComponent<DamageComponent>(collision_entity_a,
                            new DamageComponent { DamageLanded = 0, DamageReceived = damage });

                        trigger_a.triggerChecked = true;
                        trigger_b.triggerChecked = true;
                        ecb.SetComponent<TriggerComponent>(collision_entity_a, trigger_a);
                        ecb.SetComponent<TriggerComponent>(collision_entity_a, trigger_b);


                        //ecb.AddComponent<DamageComponent>(collision_entity_b,
                        // new DamageComponent { DamageLanded = hw * 10, DamageReceived = 0 });

                        Debug.Log("player a " + hw);
                    }
                }
                else if (type_b == (int)TriggerType.Ammo)//b is ammo so causes damage to entity
                {
                    //damage landed not being credited to shooter currently
                    int shooterTag = -1;
                    Entity shooter = Entity.Null;
                    if (EntityManager.HasComponent<AmmoComponent>(collision_entity_b))//ammo entity not character if true
                    {
                        shooter = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b).OwnerAmmoEntity;
                    }
                    if (shooter != Entity.Null)
                    {
                        bool isEnemy = (EntityManager.HasComponent(shooter, typeof(EnemyComponent)));

                        float damage = EntityManager.GetComponentData<GunComponent>(shooter).Damage;
                        if (shooter != collision_entity_a)
                        {
                            AmmoComponent ammo = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b); ;
                            ammo.AmmoDead = true;
                            ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);
                            ecb.AddComponent<DamageComponent>(collision_entity_a,
                                new DamageComponent { DamageLanded = 0, DamageReceived = damage });
                        }

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

