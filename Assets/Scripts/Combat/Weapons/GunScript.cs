using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Physics.Extensions;



public class GunScript : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity entity;
    [HideInInspector]
    public List<GameObject> BulletInstances = new List<GameObject>();
    public GameObject BulletPrefab;
    public GameObject WeaponPrefab;
    public AudioClip weaponAudioClip;




    private void Start()

    {

    }

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(BulletPrefab);
        gameObjects.Add(WeaponPrefab);
    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        manager = dstManager;
        this.entity = entity;
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





