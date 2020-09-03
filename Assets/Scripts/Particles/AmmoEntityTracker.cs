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
    public bool rewinding;

}

public class AmmoEntityTracker : MonoBehaviour
{
    public float ammoTime; //????? 
    public Entity ammoEntity;
    public Entity ownerAmmoEntity;
    public EntityManager manager;
    public ParticleSystem spawnPS1;
    public ParticleSystem spawnPS2;
    void Start()
    {

        //gunscript creates entity of bullet (including setting entity fields) then the start adds ammocomponent
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        manager.AddComponentData<AmmoComponent>(ammoEntity,
            new AmmoComponent
            {
                OwnerAmmoEntity = ownerAmmoEntity,
                AmmoDead = false, AmmoTimeCounter = 0, AmmoTime = ammoTime
            });
    }

    private void LateUpdate()
    {

        if (!manager.HasComponent(ammoEntity, typeof(LocalToWorld))) return;

        bool dead = manager.GetComponentData<AmmoComponent>(ammoEntity).AmmoDead;
        if (dead)
        {
            manager.DestroyEntity(ammoEntity);
            transform.localScale = Vector3.one / 100;
        }
        else
        {
            if (manager.GetComponentData<AmmoComponent>(ammoEntity).rewinding)
            {
                spawnPS1.gameObject.SetActive(false);
                spawnPS2.gameObject.SetActive(true);
            }
    

            Vector3 entityPosition = manager.GetComponentData<LocalToWorld>(ammoEntity).Position;
            entityPosition.z = 0;
            Translation translation = new Translation { Value = entityPosition };
            manager.SetComponentData(ammoEntity, translation);


            transform.position = entityPosition;
            transform.rotation = manager.GetComponentData<LocalToWorld>(ammoEntity).Rotation;

        }
    }


  
}
