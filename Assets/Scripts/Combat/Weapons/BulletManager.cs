using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Physics.Extensions;



public class BulletManager : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity entity;
    [SerializeField]
    private AudioSource audioSource;
    [HideInInspector]
    public List<GameObject> BulletInstances = new List<GameObject>();
    public Transform AmmoStartLocation;
    public GameObject BulletPrefab;
    public GameObject WeaponPrefab;
    public AudioClip weaponAudioClip;

    [Header("Weapon Ratings")]
    [SerializeField] bool randomize;
    public float AmmoTime;
    public float Strength;
    public float Damage;
    public float Rate;

    //[Header("Misc")]
    //public bool Disable;
    



    private void Start()

    {
    
    }

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(BulletPrefab);
        gameObjects.Add(WeaponPrefab);
    }

    void Generate()
    {
        float multiplier = .7f;
        Strength = UnityEngine.Random.Range(Strength * multiplier, Strength * (2 - multiplier));
        Damage = UnityEngine.Random.Range(Damage * multiplier, Damage * (2 - multiplier));
        Rate = UnityEngine.Random.Range(Rate * multiplier, Rate * (2 - multiplier));
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (randomize == true)
        {
            Generate();
        }

        dstManager.AddComponentData<GunComponent>(
            entity,
            new GunComponent()
            {
                AmmoStartPosition = new Translation(){Value = AmmoStartLocation.position},//not used because cant track bone 
                AmmoStartRotation = new Rotation(){Value = AmmoStartLocation.rotation},
                Bullet = conversionSystem.GetPrimaryEntity(BulletPrefab),
                Weapon = conversionSystem.GetPrimaryEntity(WeaponPrefab),
                Strength = Strength,
                gameStrength = Strength,
                Damage = Damage,
                Rate = Rate,
                gameDamage = Damage,
                gameRate = Rate,
                WasFiring = 0,
                IsFiring = 0

            });
        manager = dstManager;
        this.entity = entity;

    }

    public void UpdateSystem()
    {
        if (manager == default || entity == Entity.Null) return;



        if (BulletInstances.Count > 24 && BulletInstances[0].GetComponent<AmmoEntityTracker>())
        {
            Entity e = BulletInstances[0].GetComponent<AmmoEntityTracker>().ammoEntity;
            if (e != Entity.Null)
            {
                DestroyImmediate(BulletInstances[0]);
                manager.DestroyEntity(e);
                BulletInstances.RemoveAt(0);
            }
        }

    }

    public void CreateBulletInstance(Entity e)
    {
        UpdateSystem();
        GameObject go = Instantiate(BulletPrefab, AmmoStartLocation.position, AmmoStartLocation.rotation);
        //GameObject go = Instantiate(BulletPrefab, AmmoStartLocation.position, Quaternion.identity);
        BulletInstances.Add(go);
        go.GetComponent<AmmoEntityTracker>().ammoEntity = e;
        go.GetComponent<AmmoEntityTracker>().ownerAmmoEntity = entity;
        go.GetComponent<AmmoEntityTracker>().ammoTime = AmmoTime;
        if (audioSource != null)
        {
            audioSource.PlayOneShot(
                weaponAudioClip); //to do - change to ecs - EffectsManager - Effects Componnent - add start clip field = 1 then switch to 0
        }
    }
}




//public class AmmoRewindSystem : JobComponentSystem
//{



//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {

//        bool rt = false;
//        float damage = 25;

//        Entities.WithoutBurst().ForEach((Entity e, InputController inputController, ControlBarComponent controlBarComponent) =>
//        {

//            damage = controlBarComponent.value;

//            if (inputController.rightTriggerDown == true && damage < 25)
//            {
//                rt = true;
//            }
//        }
//        ).Run();


//        Entities.WithoutBurst().ForEach((Entity e,
//                ref AmmoComponent ammo, ref Translation position, ref Rotation rotation) =>
//            {
//                Entity owner = ammo.OwnerAmmoEntity;

//                if (rt == true && damage < 25)
//                {

//                    PhysicsVelocity pv = EntityManager.GetComponentData<PhysicsVelocity>(e);

//                    PhysicsVelocity velocity = new PhysicsVelocity
//                    {
//                        Linear = new float3(-pv.Linear.x, pv.Linear.y, 0) * 1.1f,
//                        Angular = float3.zero
//                    };
//                    EntityManager.SetComponentData(e, position);
//                    EntityManager.SetComponentData(e, rotation);
//                    EntityManager.SetComponentData(e, velocity);
//                    ammo.rewinding = true;
//                }
//            }


//            ).Run();




//        return default;
//    }



//}





