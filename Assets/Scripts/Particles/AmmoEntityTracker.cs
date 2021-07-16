using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public struct AmmoComponent : IComponentData
{
    public Entity OwnerAmmoEntity;
    public Entity ammoEntity;
    public bool AmmoDead;
    public float AmmoTime;
    public float AmmoTimeCounter;
    public bool DamageCausedPreviously;
    public int framesToSkip;
    public int frameSkipCounter;
    public bool bulletSpotted;
    public bool Charged;
    public bool rewinding;
    public int ammoHits;//count how many hits this ammo connected before ammoTime is zero (entity destroyed by ammo system)
    public float comboTimeAdd;
}

public class AmmoEntityTracker : MonoBehaviour, IConvertGameObjectToEntity
{
    public float ammoTime; //????? 
    [SerializeField] private float comboTimeAdd = 1.0f;
    [SerializeField]int framesToSkip = 2;
    public Entity ammoEntity;
    public Entity ownerAmmoEntity;
    EntityManager manager;
    //public ParticleSystem spawnPS;
    //public ParticleSystem spawnPS2;

    //public ParticleSystem particleCompanion;
    //public ParticleSystemRenderer rendererCompanion;
    void Start()
    {

    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //Debug.Log("convert aet ");
        manager = dstManager;
        ammoEntity = entity;
        manager.AddComponentData<AmmoComponent>(ammoEntity,
            new AmmoComponent
            {
                AmmoDead = false,
                AmmoTimeCounter = 0,
                AmmoTime = ammoTime,
                comboTimeAdd = comboTimeAdd,
                Charged = false,
                ammoEntity = entity,
                framesToSkip = framesToSkip
            });




    }
}
