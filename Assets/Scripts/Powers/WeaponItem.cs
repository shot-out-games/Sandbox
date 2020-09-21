using UnityEngine;
using Unity.Entities;


[System.Serializable]
public struct WeaponItemComponent : IComponentData
{
    public bool active;
    public int weaponType;

}




public class WeaponItem : MonoBehaviour, IConvertGameObjectToEntity
{
    public WeaponType weaponType;
    private AudioSource audioSource;


        [SerializeField]
    bool active = true;
    public Entity e;
    EntityManager manager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (manager == default || e == Entity.Null) return;
        if (manager.HasComponent<WeaponItemComponent>(e) == false) return;


        WeaponItemComponent weaponItem = manager.GetComponentData<WeaponItemComponent>(e);
        if (weaponItem.active == false)
        {
            gameObject.SetActive(false);
        };

    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        manager.AddComponentData<WeaponItemComponent>(entity, new WeaponItemComponent
        {
            active = active,
            weaponType = (int)weaponType
        });

    }


}
