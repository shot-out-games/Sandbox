using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;

[System.Serializable]
public struct AttachWeaponComponent : IComponentData
{
    public int attachedWeaponSlot;
    public int attachWeaponType;
    public int attachSecondaryWeaponType;
    public int weaponsAvailableCount;


}

public class AttachWeapon : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<Weapons> weaponsList;//actual weapon
    [SerializeField] private int weaponIndex = 0;//index of weapons list to start with
    private EntityManager manager;
    public Entity e;

    private bool attachedWeapon;



    void Start()
    {


    }


    public void DeactivateWeapons()
    {
        if (weaponsList.Count == 0) return;//inactive to start 
        for (int i = 0; i < weaponsList.Count; i++)
        {
            weaponsList[i].weaponGameObject.SetActive(false);
        }

    }




    public void AttachAvailableWeapons(int _weaponIndex)
    {
        if (weaponsList.Count == 0) return;//


        for (int i = 0; i < weaponsList.Count; i++)
        {

            Weapons weapon = weaponsList[i];
            if (i == weaponIndex)
            {
                var attached = manager.GetComponentData<AttachWeaponComponent>(e);
                attached.attachedWeaponSlot = i;
                attached.attachWeaponType = (int)weaponsList[i].weaponType;
                manager.SetComponentData<AttachWeaponComponent>(e, attached);
            }

            if (weaponsList[i].secondaryWeapon == true)
            {

                var attached = manager.GetComponentData<AttachWeaponComponent>(e);
                attached.attachSecondaryWeaponType = (int)weaponsList[i].weaponType;
                manager.SetComponentData<AttachWeaponComponent>(e, attached);

            }



        }

        weaponIndex = _weaponIndex;


    }




    public void AttachPickWeapons(int _weaponIndex)
    {
        if (weaponsList.Count == 0) return;//

        Weapons weapon = weaponsList[_weaponIndex];
        weaponsList[_weaponIndex].isAttached = true;
        var attached = manager.GetComponentData<AttachWeaponComponent>(e);
        attached.attachedWeaponSlot = _weaponIndex;
        attached.attachWeaponType = (int)weaponsList[_weaponIndex].weaponType;
        manager.SetComponentData<AttachWeaponComponent>(e, attached);
        weaponsList[_weaponIndex].weaponGameObject.SetActive(true);
        weaponIndex = _weaponIndex;
        AttachAvailableWeapons(weaponIndex);



    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new AttachWeaponComponent
            {
                attachedWeaponSlot = 0,//-1 is buggy but needed for pickup weapon IK
                attachWeaponType = weaponIndex,
                weaponsAvailableCount = weaponsList.Count
            });
        e = entity;
        manager = dstManager;

    }




}


public class AttachWeaponSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                ref AttachWeaponComponent AttachWeaponComponent,
                in AttachWeapon attachWeapon,
                in InputController inputController
            ) =>
            {

                int currentSlot = AttachWeaponComponent.attachedWeaponSlot;
                int weaponCount = attachWeapon.weaponsList.Count;
                //if (inputController.leftBumperPressed && currentSlot >= 0)
                if (currentSlot >= 0)
                {
                    for (int i = 0; i < weaponCount; i++)
                    {
                        Weapons weapon = attachWeapon.weaponsList[i];
                        currentSlot++;
                        if (currentSlot >= weaponCount) currentSlot = 0;
                        if (weapon.isAttached == true)
                        {
                            attachWeapon.AttachPickWeapons(currentSlot);
                            break;
                        }

                    }
                }

            }
        ).Run();




        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                ref AttachWeaponComponent AttachWeaponComponent,
                in AttachWeapon attachWeapon, in EnemyComponent enemyComponent
            ) =>
            {
                int currentSlot = AttachWeaponComponent.attachedWeaponSlot;
                int weaponCount = attachWeapon.weaponsList.Count;
                for (int i = 0; i < weaponCount; i++)
                {
                    Weapons weapon = attachWeapon.weaponsList[i];
                    currentSlot++;
                    if (currentSlot >= weaponCount) currentSlot = 0;
                    if (weapon.isAttached == true)
                    {
                        attachWeapon.AttachPickWeapons(currentSlot);
                        break;
                    }

                }

            }
        ).Run();



        return default;
    }

}

