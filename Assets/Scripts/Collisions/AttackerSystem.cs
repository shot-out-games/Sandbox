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
                (HealthBar healthBar, CollisionComponent collisionComponent,
                RatingsComponent ratingsComponent, Animator animator, in Entity entity, in Transform transform) =>
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


                //float hw = animator.GetFloat("HitWeight");

                if (type_b == (int)TriggerType.Ammo)//b is ammo so causes damage to entity
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
                        bool isEnemy = (EntityManager.HasComponent(shooter, typeof(EnemyComponent))) ;

                        float damage = EntityManager.GetComponentData<GunComponent>(shooter).Damage;
                        //bool shootSelf = false;
                        //if (EntityManager.HasComponent<RewindComponent>(shooter))
                        //{
                           // shootSelf = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b).rewinding;
                        //}

                        //if(shootSelf == true && shooter == collision_entity_a)
                        //{
                        //    AmmoComponent ammo = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b); ;
                        //    ammo.AmmoDead = true;
                        //    ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);
                        //    ecb.AddComponent<DamageComponent>(collision_entity_a,
                        //        new DamageComponent { DamageLanded = 0, DamageReceived = damage });
                        //}
                        if (shooter != collision_entity_a)
                        {
                            //float f = 10;
                            //if (isEnemy) f = 1;
                            AmmoComponent ammo = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b); ;
                            ammo.AmmoDead = true;
                            ecb.SetComponent<AmmoComponent>(collision_entity_b, ammo);
                            //                            ecb.AddComponent<DamageComponent>(collision_entity_a,
                            //                             new DamageComponent { DamageLanded = 0, DamageReceived = damage / f });
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

