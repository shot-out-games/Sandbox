using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Physics.Extensions;

//public class AmmoRatings
//{
//    bool randomize;
//    public float AmmoTime;
//    public float Strength;
//    public float Damage;
//    public float Rate;

//}

public class BulletManager : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity entity;
    [SerializeField]
    private AudioSource audioSource;
    [HideInInspector]
    public List<GameObject> AmmoInstances = new List<GameObject>();
    public Transform AmmoStartLocation;
    public GameObject PrimaryAmmoPrefab;
    public GameObject SecondaryAmmoPrefab;
    public List<GameObject> AmmoPrefabList = new List<GameObject>();

    //public GameObject WeaponPrefab;
    public AudioClip weaponAudioClip;

    [Header("Ammo Ratings")]
    [SerializeField]
    bool randomize;
    float AmmoTime;
    float Strength;
    float Damage;
    float Rate;

    //[Header("Misc")]
    //public bool Disable;



    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(PrimaryAmmoPrefab);
        gameObjects.Add(SecondaryAmmoPrefab);
        //gameObjects.Add(WeaponPrefab);
    }

    void Generate(bool randomize)
    {
        if (randomize)
        {
            float multiplier = .7f;
            Strength = UnityEngine.Random.Range(Strength * multiplier, Strength * (2 - multiplier));
            Damage = UnityEngine.Random.Range(Damage * multiplier, Damage * (2 - multiplier));
            Rate = UnityEngine.Random.Range(Rate * multiplier, Rate * (2 - multiplier));
        }
        else
        {
            Strength = PrimaryAmmoPrefab.GetComponent<AmmoData>().Strength;
            Damage = PrimaryAmmoPrefab.GetComponent<AmmoData>().Damage;
            Rate = PrimaryAmmoPrefab.GetComponent<AmmoData>().Rate;
            Debug.Log("Str " + Strength);
        }


    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //if (randomize == true)
        //{
            Generate(randomize);
        //}

        dstManager.AddComponentData<GunComponent>(
            entity,
            new GunComponent()
            {
                AmmoStartPosition = new Translation(){Value = AmmoStartLocation.position},//not used because cant track bone 
                AmmoStartRotation = new Rotation(){Value = AmmoStartLocation.rotation},
                PrimaryAmmo = conversionSystem.GetPrimaryEntity(PrimaryAmmoPrefab),
                SecondaryAmmo = conversionSystem.GetPrimaryEntity(SecondaryAmmoPrefab),
                //Weapon = conversionSystem.GetPrimaryEntity(WeaponPrefab),
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

    //public void UpdateSystem()
    //{
    //    if (manager == default || entity == Entity.Null) return;

    //    Debug.Log("ammo instances " + AmmoInstances.Count);

    //    if (AmmoInstances.Count > 24 && AmmoInstances[0].GetComponent<AmmoEntityTracker>())
    //    {
    //        Entity e = AmmoInstances[0].GetComponent<AmmoEntityTracker>().ammoEntity;
    //        if (e != Entity.Null)
    //        {
    //            DestroyImmediate(AmmoInstances[0]);
    //            manager.DestroyEntity(e);
    //            AmmoInstances.RemoveAt(0);
    //        }
    //    }

    //}

    //public void CreatePrimaryAmmoInstance(Entity e)
    //{
    //    UpdateSystem();
    //    GameObject go = Instantiate(PrimaryAmmoPrefab, AmmoStartLocation.position, AmmoStartLocation.rotation);
    //    //GameObject go = Instantiate(BulletPrefab, AmmoStartLocation.position, Quaternion.identity);
    //    AmmoInstances.Add(go);
    //    go.GetComponent<AmmoEntityTracker>().ammoEntity = e;
    //    go.GetComponent<AmmoEntityTracker>().ownerAmmoEntity = entity;
    //    go.GetComponent<AmmoEntityTracker>().ammoTime = AmmoTime;
    //    if (audioSource != null)
    //    {
    //        audioSource.PlayOneShot(
    //            weaponAudioClip); //to do - change to ecs - EffectsManager - Effects Componnent - add start clip field = 1 then switch to 0
    //    }
    //}
}


