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


                //Debug.Log("ta " + type_a + " tb " + type_b);
                //Debug.Log("ea " + collision_entity_a + " eb " + collision_entity_b);





                bool attackerIsA = type_b == (int)TriggerType.Chest &&
                                   (type_a == (int)TriggerType.LeftHand || type_a == (int)TriggerType.RightHand
                                                                        || type_a == (int)TriggerType.LeftFoot ||
                                                                        type_a == (int)TriggerType.RightFoot);


                bool attackerIsB = type_a == (int)TriggerType.Chest &&
                                   (type_b == (int)TriggerType.LeftHand || type_b == (int)TriggerType.RightHand
                                                                        || type_b == (int)TriggerType.LeftFoot ||
                                                                        type_b == (int)TriggerType.RightFoot);

                if (attackerIsA) counta = counta + 1;
                if (attackerIsB) countb = countb + 1;

                Debug.Log("c a " + counta);

                Debug.Log("c b " + countb);



                float hw = animator.GetFloat("HitWeight");



                if (attackerIsA == false && attackerIsB == true) return;

                else

                if (
                    attackerIsA == true

                && hw > .03
                )
                {

                    var trigger_a = EntityManager.GetComponentData<CheckedComponent>(collision_entity_a);
                    var trigger_b = EntityManager.GetComponentData<CheckedComponent>(collision_entity_b);

                    bool triggerChecked_a = trigger_a.collisionChecked;
                    bool triggerChecked_b = trigger_b.collisionChecked;
                    float hitPower = 1;

                    if (triggerChecked_a == false && triggerChecked_b == false)
                    {
                        if (attackerIsA)
                        {
                            bool hasMelee = EntityManager.HasComponent<MeleeComponent>(collision_entity_a);
                            if (hasMelee)
                            {
                                hitPower = EntityManager.GetComponentData<MeleeComponent>(collision_entity_a).gameHitPower;
                            }

                            float damage = hitPower * hw;

                            ecb.AddComponent<DamageComponent>(collision_entity_b,
                                new DamageComponent { DamageLanded = 0, DamageReceived = damage });

                            trigger_a.collisionChecked = true;
                            trigger_b.collisionChecked = true;
                            ecb.SetComponent<CheckedComponent>(collision_entity_a, trigger_a);
                            ecb.SetComponent<CheckedComponent>(collision_entity_a, trigger_b);

                        }
                        //else if (attackerIsB)
                        //{

                        //    bool hasMelee = EntityManager.HasComponent<MeleeComponent>(collision_entity_b);
                        //    if (hasMelee)
                        //    {
                        //        hitPower = EntityManager.GetComponentData<MeleeComponent>(collision_entity_b).gameHitPower;
                        //    }

                        //    float damage = hitPower * hw;

                        //    ecb.AddComponent<DamageComponent>(collision_entity_a,
                        //        new DamageComponent { DamageLanded = 0, DamageReceived = damage });


                        //}

                        //if (attackerIsA || attackerIsB)
                        //{
                        //}

                    }


                }





                if (type_b == (int)TriggerType.Ammo) //b is ammo so causes damage to entity
                {
                    //damage landed not being credited to shooter currently
                    int shooterTag = -1;
                    Entity shooter = Entity.Null;
                    if (EntityManager.HasComponent<AmmoComponent>(collision_entity_b)
                    ) //ammo entity not character if true
                    {
                        shooter = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b).OwnerAmmoEntity;
                    }

                    if (shooter != Entity.Null)
                    {
                        bool isEnemy = (EntityManager.HasComponent(shooter, typeof(EnemyComponent)));

                        float damage = EntityManager.GetComponentData<GunComponent>(shooter).gameDamage;
                        if (shooter != collision_entity_a)
                        {
                            AmmoComponent ammo = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b);
                            ;
                            ammo.AmmoDead = true;
                            ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);
                            ecb.AddComponent<DamageComponent>(collision_entity_a,
                                new DamageComponent { DamageLanded = 0, DamageReceived = damage, StunLanded = damage });
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

