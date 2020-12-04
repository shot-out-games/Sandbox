using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public struct AmmoComponent : IComponentData
{
    public Entity OwnerAmmoEntity;
    public bool AmmoDead;
    public float AmmoTime;
    public float AmmoTimeCounter;
    public bool DamageCausedPreviously;
    public bool rewinding;
}

public class AmmoEntityTracker : MonoBehaviour, IConvertGameObjectToEntity
{
    public float ammoTime; //????? 
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
                AmmoTime = ammoTime
            });




    }
}
