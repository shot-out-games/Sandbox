//using Rewired;
//using Unity.Entities;
//using Unity.Jobs;
//using UnityEngine;
//using Unity.Collections;


//public class RewindDamageSystem : JobComponentSystem
//{


//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


//        Entities.WithoutBurst().ForEach(
//                //(HealthBar healthBar, DynamicBuffer<CollisionComponent> collisionComponent,
//                (HealthBar healthBar, CollisionComponent collisionComponent,
//                RatingsComponent ratingsComponent, Animator animator, in Entity entity, in Transform transform) =>
//            {
//                bool blocked = false;
//                bool dead = false;
//                if (EntityManager.HasComponent<DeadComponent>(entity))
//                {
//                    dead = EntityManager.GetComponentData<DeadComponent>(entity).isDead;
//                }

//                //if (collisionComponent.Length == 0 || dead) return;
//                if (dead) return;



//                int type_a = collisionComponent.Part_entity;
//                int type_b = collisionComponent.Part_other_entity;
//                Entity collision_entity_a = collisionComponent.Character_entity;
//                Entity collision_entity_b = collisionComponent.Character_other_entity;


//                int teammate_a = ratingsComponent.tag;//used to make sure not teammates do not collide
//                int teammate_b = 0;

//                bool is_character = EntityManager.HasComponent<RatingsComponent>(collision_entity_b);
//                if (is_character)
//                {
//                    teammate_b = EntityManager.GetComponentData<RatingsComponent>(collision_entity_b).tag;
//                }

//                float hw = animator.GetFloat("HitWeight");

//                if (type_b == (int)TriggerType.Ammo)//b is ammo so causes damage to entity
//                {
//                    //damage landed not being credited to shooter currently
//                    int shooterTag = -1;
//                    float damage = 1;
//                    Entity shooter = Entity.Null;
//                    if (EntityManager.HasComponent<AmmoComponent>(collision_entity_b))//ammo entity not character if true
//                    {
//                        shooter = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b).OwnerAmmoEntity;
//                        blocked = EntityManager.GetComponentData<BlockComponent>(collision_entity_b).blocked;
//                        teammate_b = EntityManager.GetComponentData<RatingsComponent>(shooter).tag;
//                        shooterTag = teammate_b;
//                    }

//                    //if (EntityManager.HasComponent<GunComponent>(entity) && shooter != Entity.Null)
//                    //blocked = true;
//                    if (shooter != Entity.Null)
//                    {

//                        bool shootSelf = false;

//                        if (shooterTag == 2 && EntityManager.HasComponent(shooter, typeof(RewindComponent)) )
//                        {
//                            //if (EntityManager.HasComponent(coll, typeof(AmmoComponent)))
//                            //{
//                                bool rewinding = EntityManager.GetComponentData<AmmoComponent>(collision_entity_b).rewinding;
//                                if (rewinding) shootSelf = true;
//                            //}
//                        }


//                        damage = teammate_a == teammate_b && shootSelf == false ? 0 : EntityManager.GetComponentData<GunComponent>(shooter).Damage;

//                        Debug.Log("r1  " + shootSelf + " dam " + damage);

//                        if (blocked && EntityManager.HasComponent<ControlBarComponent>(shooter))
//                        {
//                            ControlBarComponent controlBarComponent =
//                                EntityManager.GetComponentData<ControlBarComponent>(shooter);
//                            controlBarComponent.value = controlBarComponent.value * .5f;
//                            EntityManager.SetComponentData(shooter, controlBarComponent);
//                            //Debug.Log("value");
//                        }
//                        //blocked = true;
//                        //if ((shooterTag == 1) || shooterTag != 1)
//                        //{
//                        if (shootSelf == false)
//                        {
//                            ecb.AddComponent<DamageComponent>(entity, new DamageComponent { DamageLanded = 0, DamageReceived = damage });
//                            ecb.AddComponent<DamageComponent>(shooter, new DamageComponent { DamageLanded = damage, DamageReceived = 0 });
//                        }
//                        else
//                        {
//                            ecb.AddComponent<DamageComponent>(shooter, new DamageComponent { DamageLanded = 0, DamageReceived = damage });
//                            ecb.AddComponent<DamageComponent>(entity, new DamageComponent { DamageLanded = damage, DamageReceived = 0 });
//                        }



//                        //}
//                        StatsComponent stats = EntityManager.GetComponentData<StatsComponent>(shooter);//cant put in query because need shooter
//                        stats.shotsLanded += 1;
//                        SkillTreeComponent skills = EntityManager.GetComponentData<SkillTreeComponent>(shooter);//cant put in query because need shooter
//                        skills.CurrentLevelXp += 10;
//                        ecb.SetComponent(shooter, stats);
//                        ecb.SetComponent(shooter, skills);

//                    }
//                }
//                else if (type_a == (int)TriggerType.Ammo)//b is ammo so causes damage to entity
//                {
//                    //damage landed not being credited to shooter currently
//                    int shooterTag = -1;
//                    float damage = 1;
//                    Entity shooter = Entity.Null;
//                    if (EntityManager.HasComponent<AmmoComponent>(collision_entity_a))//ammo entity not character if true
//                    {
//                        shooter = EntityManager.GetComponentData<AmmoComponent>(collision_entity_a).OwnerAmmoEntity;
//                        blocked = EntityManager.GetComponentData<BlockComponent>(collision_entity_a).blocked;
//                        teammate_b = EntityManager.GetComponentData<RatingsComponent>(shooter).tag;
//                        shooterTag = teammate_b;
//                    }


//                    //if (EntityManager.HasComponent<GunComponent>(entity) && shooter != Entity.Null)
//                    if (shooter != Entity.Null)
//                    {
//                        bool shootSelf = false;


//                        if (shooterTag == 2 && EntityManager.HasComponent(shooter, typeof(RewindComponent)))
//                        {
//                            //if (EntityManager.HasComponent(entity, typeof(AmmoComponent)))
//                            //{
//                                bool rewinding = EntityManager.GetComponentData<AmmoComponent>(collision_entity_a).rewinding;
//                                if (rewinding) shootSelf = true;
//                                Debug.Log("r2  " + shootSelf);
//                            //}
//                        }


//                        damage = teammate_a == teammate_b && shootSelf == false ? 0 : EntityManager.GetComponentData<GunComponent>(shooter).Damage;

//                        //blocked = true;
//                        //if ((blocked == true && shooterTag == 1) || shooterTag != 1)
//                        //{

//                        if (shootSelf == false)
//                        {
//                            ecb.AddComponent<DamageComponent>(entity, new DamageComponent { DamageLanded = 0, DamageReceived = damage });
//                            ecb.AddComponent<DamageComponent>(shooter, new DamageComponent { DamageLanded = damage, DamageReceived = 0 });
//                        }
//                        else
//                        {
//                            ecb.AddComponent<DamageComponent>(shooter, new DamageComponent { DamageLanded = 0, DamageReceived = damage });
//                            ecb.AddComponent<DamageComponent>(entity, new DamageComponent { DamageLanded = damage, DamageReceived = 0 });
//                        }

//                        //}
//                        StatsComponent stats = EntityManager.GetComponentData<StatsComponent>(shooter);//cant put in query because need shooter
//                        stats.shotsLanded += 1;
//                        SkillTreeComponent skills = EntityManager.GetComponentData<SkillTreeComponent>(shooter);//cant put in query because need shooter
//                        skills.CurrentLevelXp += 10;
//                        ecb.SetComponent(shooter, stats);
//                        ecb.SetComponent(shooter, skills);

//                    }
//                }
//                else if (hw > 0 && teammate_a != teammate_b)
//                {
//                    ecb.AddComponent<DamageComponent>(collision_entity_b, new DamageComponent { DamageLanded = 0, DamageReceived = hw });
//                    ecb.AddComponent<DamageComponent>(entity, new DamageComponent { DamageLanded = hw, DamageReceived = 0 });
//                }

//                ecb.RemoveComponent<CollisionComponent>(entity);




//            }
//            ).Run();









//        Entities.WithoutBurst().ForEach(
//            (Animator animator, DamageComponent damageComponent) =>
//            {

//                if (damageComponent.DamageReceived > .000001)
//                {
//                    animator.SetInteger("HitReact", 1);

//                }


//            }
//        ).Run();


//        ecb.Playback(EntityManager);
//        ecb.Dispose();


//        return default;
//    }

//}

