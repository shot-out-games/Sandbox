using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;



public class NpcSpawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity entity;
    public GameObject NpcGameObject;
    //public List<Entity> NpcEntityList = new List<Entity>();
    //public GameObject NpcPrefab;
    //[SerializeField] int npcInstancesCount = 36;

    [SerializeField]
    private bool checkWinCondition = true;

    [SerializeField]
    private bool checkLossCondition = true;

    public int spawnerIndex { get; set; }



    private void Start()
    {
        Debug.Log("start spawner");
    }

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {

        //for (int i = 0; i < NpcGameObjects.Length; i++)
        //{
           // Debug.Log("npc " + NpcGameObjects[i]);
            referencedPrefabs.Add(NpcGameObject);
        //}
    }


    public void Spawned(string holeName)
    {
        //holeName = holeName.Replace("robot", "").Replace(" ", "");
        holeName = holeName.Replace("hole", "");
        spawnerIndex = System.Int32.Parse(holeName);
        Debug.Log("npc saved id  " + holeName);
    }


    public void AddComponents(Entity npcEntity)
    {
        Debug.Log("convert npc spawner " + npcEntity);

        manager.AddComponentData(npcEntity, new CharacterSaveComponent { saveIndex = spawnerIndex });


        manager.AddComponentData<NpcComponent>(npcEntity, new NpcComponent
            {
                e = npcEntity
            }
        );

        manager.AddComponentData(npcEntity, new Pause { value = 0 });


        manager.AddComponentData(npcEntity,
            new WinnerComponent
            {
                active = true,
                goalCounter = 0,
                goalCounterTarget = 0,//ie how many players you have to save - usually zero
                targetReached = false,
                endGameReached = false,
                checkWinCondition = checkWinCondition
            }
        );

        manager.AddComponentData(npcEntity,
            new LevelCompleteComponent
            {
                active = true,
                goalCounter = 0,
                goalCounterTarget = 0,//ie how many players you have to save - usually zero
                targetReached = false,
                checkWinCondition = checkWinCondition
            }
        );


        manager.AddComponentData(npcEntity, new DeadComponent
            {
                tag = 3,
                isDead = false,
                checkLossCondition = checkLossCondition

            }
        );


        manager.AddComponentData(npcEntity, new SkillTreeComponent()
            {
                e = npcEntity,
                availablePoints = 0,
                SpeedPts = 0,
                PowerPts = 0,
                ChinPts = 0,
                baseSpeed = 0,
                CurrentLevel = 1,
                CurrentLevelXp = 0,
                PointsNextLevel = 10

            }

        );




    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {


        manager = dstManager;
        this.entity = entity;

        Entity e = conversionSystem.GetPrimaryEntity(NpcGameObject);
        AddComponents(e);

        //manager.Instantiate(e);

    }

    void Update()
    {
        //AmmoStartLocation.position = transform.InverseTransformDirection(AmmoStartLocation.position);

        //var gunComponent = manager.GetComponentData<GunComponent>(entity);


    }

    public void CreateRobotInstance(Entity e)
    {

        //Entity e = EntityManager.Instantiate(RootPrefab);


        //GameObject go = Instantiate(RobotPrefab);
        //RobotInstances.Add(go);

    }







    //public void CreateBulletInstance(Entity e)
    //{
    //    GameObject go = Instantiate(BulletPrefab, AmmoStartLocation.position, AmmoStartLocation.rotation);
    //    BulletInstances.Add(go);
    //    go.GetComponent<AmmoEntityTracker>().ammoEntity = e;
    //    go.GetComponent<AmmoEntityTracker>().ownerAmmoEntity = entity;
    //    go.GetComponent<AmmoEntityTracker>().ammoTime = AmmoTime;
    //    audioSource.PlayOneShot(weaponAudioClip);//to do - change to ecs - EffectsManager - Effects Componnent - add start clip field = 1 then switch to 0

    //}



//}


//public class GunAmmoHandlerSystem : JobComponentSystem
//{



//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {


//        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
//        float dt = UnityEngine.Time.fixedDeltaTime;//gun duration


//        Entities.WithoutBurst().WithStructuralChanges().ForEach(
//            (ref GunComponent gun, ref LocalToWorld gunTransform, ref StatsComponent statsComponent,
//                ref Rotation gunRotation, in GunScript gunScript, in Entity entity, in AttachWeaponComponent attachWeapon) =>
//            {
//                if (gun.IsFiring == 0 || attachWeapon.attachedWeaponSlot < 0 ||
//                    attachWeapon.attachWeaponType != (int)WeaponType.Gun &&
//                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Gun
//                    )
//                {
//                    gun.IsFiring = 0;
//                    gun.Duration = 0;
//                    gun.WasFiring = 0;
//                    return;
//                }


//                if (EntityManager.GetComponentData<Pause>(entity).value == 1) return;
//                if (EntityManager.GetComponentData<DeadComponent>(entity).isDead) return;

//                gun.Duration += dt;
//                if ((gun.Duration > gun.Rate) || (gun.WasFiring == 0))
//                {
//                    if (gun.Bullet != null)
//                    {
//                        statsComponent.shotsFired += 1;
//                        Entity e = EntityManager.Instantiate(gun.Bullet);
//                        Translation position = new Translation { Value = gunScript.AmmoStartLocation.transform.position };
//                        Rotation rotation = new Rotation { Value = gunScript.AmmoStartLocation.rotation };
//                        PhysicsVelocity velocity = new PhysicsVelocity
//                        {
//                            Linear = gunScript.AmmoStartLocation.forward * gun.Strength,
//                            Angular = float3.zero
//                        };
//                        EntityManager.SetComponentData(e, position);
//                        EntityManager.SetComponentData(e, rotation);
//                        EntityManager.SetComponentData(e, velocity);
//                        gunScript.CreateBulletInstance(e);


//                    }
//                    gun.Duration = 0;
//                }




//                gun.WasFiring = 1;
//            }
//        ).Run();


//        return default;
//    }



//}

}