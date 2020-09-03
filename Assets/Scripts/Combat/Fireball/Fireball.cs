using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;



public struct FireballWeaponComponent : IComponentData
{
    public Entity Ammo;
    public float AmmoTime;
    public Entity Weapon;//Hand
    public float Strength;
    public float Rate;
    public float Duration;
    public int WasFiring;
    public int IsFiring;
    public float3 AmmoStartLocation;
}

public class Fireball : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public List<GameObject> FireballInstances = new List<GameObject>();
    public Transform AmmoStartLocation;
    public GameObject FireballAmmoPrefab;
    public GameObject Weapon;//not required
    public float Strength;
    public float Rate;
    public float ammoTime;
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip audioClip;
    private EntityManager _entityManager;
    private Entity _entity;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (GetComponent<AudioSource>() == null) return;
        audioSource = GetComponent<AudioSource>();
    }
    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(FireballAmmoPrefab);
        gameObjects.Add(Weapon);
    }


    void Update()
    {
        //AmmoStartLocation.position = transform.InverseTransformDirection(AmmoStartLocation.position);
        if (FireballInstances.Count > 6)
        {
            Entity e = FireballInstances[0].GetComponent<AmmoEntityTracker>().ammoEntity;
            DestroyImmediate(FireballInstances[0]);
            _entityManager.DestroyEntity(e);
            FireballInstances.RemoveAt(0);

        }

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        _entity = entity;
        _entityManager = dstManager;


        dstManager.AddComponentData<FireballWeaponComponent>(
            entity,
            new FireballWeaponComponent()
            {
                AmmoStartLocation = AmmoStartLocation.position,
                Ammo = conversionSystem.GetPrimaryEntity(FireballAmmoPrefab),
                Weapon = conversionSystem.GetPrimaryEntity(Weapon),
                Strength = Strength,
                Rate = Rate,
                WasFiring = 0,
                IsFiring = 0

            });
    }

    public void StartShot()
    {
        rb.isKinematic = true;
        FireballWeaponComponent fireballWeaponComponent =
            _entityManager.GetComponentData<FireballWeaponComponent>(_entity);
        fireballWeaponComponent.IsFiring = 1;
        _entityManager.SetComponentData(_entity, fireballWeaponComponent);
        animator.SetBool("Fireball", false);

    }

    public void EndShot()
    {
        rb.isKinematic = false;

    }


    public void CreateBulletInstance(Entity e)
    {
        audioSource.PlayOneShot(audioClip);
        GameObject go = Instantiate(FireballAmmoPrefab, AmmoStartLocation.position, AmmoStartLocation.rotation);
        FireballInstances.Add(go);
        go.GetComponent<AmmoEntityTracker>().ammoEntity = e;
        go.GetComponent<AmmoEntityTracker>().ownerAmmoEntity = _entity;
        go.GetComponent<AmmoEntityTracker>().ammoTime = ammoTime;

    }
}



public class FireballHandlerSystem : JobComponentSystem
{



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {


        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        float dt = UnityEngine.Time.fixedDeltaTime;

        Entities.WithoutBurst().WithStructuralChanges().ForEach(
            (ref FireballWeaponComponent hand, ref LocalToWorld handTransform, ref Rotation handRotation, in Fireball fireballScript, in AttachWeaponComponent attachWeapon) =>
            {
                if (hand.IsFiring == 0 || attachWeapon.attachedWeaponSlot < 0 ||
                    attachWeapon.attachWeaponType != (int)WeaponType.Fireball &&
                    attachWeapon.attachSecondaryWeaponType != (int)WeaponType.Fireball
                )
                {
                    hand.IsFiring = 0;
                    hand.Duration = 0;
                    hand.WasFiring = 0;
                    return;
                }

                hand.Duration += dt;
                if ((hand.Duration > hand.Rate) || (hand.WasFiring == 0))
                {
                    if (hand.Ammo != null)
                    {
                        Entity e = EntityManager.Instantiate(hand.Ammo);
                        Translation position = new Translation { Value = fireballScript.AmmoStartLocation.transform.position };
                        Rotation rotation = new Rotation { Value = fireballScript.AmmoStartLocation.rotation };
                        PhysicsVelocity velocity = new PhysicsVelocity
                        {
                            Linear = fireballScript.AmmoStartLocation.forward * hand.Strength,
                            Angular = float3.zero
                        };
                        EntityManager.SetComponentData(e, position);
                        EntityManager.SetComponentData(e, rotation);
                        EntityManager.SetComponentData(e, velocity);

                        fireballScript.CreateBulletInstance(e);


                    }
                    hand.Duration = 0;
                }
                hand.WasFiring = 1;
            }
        ).Run();

        return default;
    }



}













