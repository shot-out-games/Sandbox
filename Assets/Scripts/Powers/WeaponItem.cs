using RootMotion.FinalIK;
using UnityEngine;
using Unity.Entities;


//[System.Serializable]
//public struct WeaponItemComponent : IComponentData
//{
//    public Entity e;
//    public bool active;
//    public int weaponType;
//    public bool pickedUp;
//}

[System.Serializable]
public struct WeaponItemComponent : IBufferElementData
{
    public Entity e;
    public bool active;
    public int weaponType;
    public bool pickedUp;
}




public class WeaponItem : MonoBehaviour, IConvertGameObjectToEntity
{

    public Weapons weaponData;

    //public WeaponType weaponType; //in weapondata
    private AudioSource audioSource;
    [HideInInspector] public InteractionObject interactionObject;//set from wea

    [SerializeField]
    bool active = true;
    public Entity e;
    EntityManager manager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactionObject = GetComponent<InteractionObject>();
    }


    void Update()
    {
        if (manager == default || e == Entity.Null) return;
        if (manager.HasComponent<WeaponItemComponent>(e) == false) return;


        //WeaponItemComponent weaponItem = manager.GetComponentData<WeaponItemComponent>(e);
        //if (weaponItem.active == false)
        //{
        //    gameObject.SetActive(false);
        //};

    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        var buffer =  manager.AddBuffer<WeaponItemComponent>(entity);
        buffer.Add
        (
            new WeaponItemComponent()
            {
                e = entity,
                active = active,
                weaponType = (int)weaponData.weaponType
            }
        );


        //manager.Add<WeaponItemComponent>(entity, new WeaponItemComponent
        //{
        //    active = active,
        //    weaponType = (int)weaponData.weaponType
        //});
        //manager.AddComponentData<WeaponItemComponent>(entity, new WeaponItemComponent
        //{
        //    active = active,
        //    weaponType = (int)weaponData.weaponType
        //});

    }


}
