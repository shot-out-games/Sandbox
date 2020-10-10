using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;

[System.Serializable]
public struct AttachWeaponComponent : IComponentData
{
    public bool isAttached;//na
    public int attachedWeaponSlot;
    public int attachWeaponType;
    public int attachSecondaryWeaponType;
    public int weaponsAvailableCount;


}

public class WeaponManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public Weapons primaryWeapon;
    public Weapons secondaryWeapon;

    public List<Weapons> weaponsList;//actual weapon
    private int weaponIndex = 0;//index of weapons list to start with
    private EntityManager manager;
    public Entity e;
    [HideInInspector]
    public bool primaryAttached = false;

    private bool attachedWeapon;

    void Start()
    {
        if (primaryWeapon != null)
        {
            if (primaryWeapon.isAttachedAtStart)
            {
                AttachPrimaryWeapon();
            }
        }
        if (secondaryWeapon != null)
        {
            if (secondaryWeapon.isAttachedAtStart)
            {
                AttachSecondaryWeapon();
            }
        }
    }


    public void DeactivateWeapons()
    {
        if (weaponsList.Count == 0) return;//inactive to start 
        for (int i = 0; i < weaponsList.Count; i++)
        {
            weaponsList[i].weaponGameObject.SetActive(false);
        }

    }

    public void AttachPrimaryWeapon()
    {
        primaryAttached = true;
        primaryWeapon.weaponGameObject.transform.SetParent(primaryWeapon.weaponLocation);
        primaryWeapon.weaponGameObject.transform.localPosition = Vector3.zero;
        primaryWeapon.weaponGameObject.transform.localRotation = Quaternion.identity;


    }

    public void DetachPrimaryWeapon()
    {
        if (primaryWeapon != null)
        {
            primaryAttached = false;
            //primaryWeapon.weaponGameObject.SetActive(false);
            Debug.Log("pr " + primaryWeapon.weaponGameObject);
            primaryWeapon.weaponGameObject.transform.SetParent(null);
        }
        //primaryWeapon.weaponGameObject.transform.localPosition = Vector3.zero;
        //primaryWeapon.weaponGameObject.transform.localRotation = Quaternion.identity;


    }

    public void AttachSecondaryWeapon()
    {
        secondaryWeapon.weaponGameObject.transform.SetParent(secondaryWeapon.weaponLocation);
        secondaryWeapon.weaponGameObject.transform.localPosition = Vector3.zero;
        secondaryWeapon.weaponGameObject.transform.localRotation = Quaternion.identity;


    }

    public void AttachAvailableWeapons(int _weaponIndex)
    {
        //if (weaponsList.Count == 0) return; //
        //for (int i = 0; i < weaponsList.Count; i++)
        //{
        //    Weapons weapon = weaponsList[i];
        //    if (i == weaponIndex)
        //    {
        //        var attached = manager.GetComponentData<AttachWeaponComponent>(e);
        //        attached.attachedWeaponSlot = i;
        //        attached.attachWeaponType = (int) weaponsList[i].weaponType;
        //        manager.SetComponentData<AttachWeaponComponent>(e, attached);
        //    }

        //    if (weaponsList[i].secondaryWeapon == true)
        //    {
        //        var attached = manager.GetComponentData<AttachWeaponComponent>(e);
        //        attached.attachSecondaryWeaponType = (int) weaponsList[i].weaponType;
        //        manager.SetComponentData<AttachWeaponComponent>(e, attached);
        //    }
        //}

        //weaponIndex = _weaponIndex;
    }

    public void AttachPickWeapons(int _weaponIndex)
    {
        //if (weaponsList.Count == 0) return; //
        //Weapons weapon = weaponsList[_weaponIndex];
        //weaponsList[_weaponIndex].isAttached = true;
        //var attached = manager.GetComponentData<AttachWeaponComponent>(e);
        //attached.attachedWeaponSlot = _weaponIndex;
        //attached.attachWeaponType = (int) weaponsList[_weaponIndex].weaponType;
        //manager.SetComponentData<AttachWeaponComponent>(e, attached);


        ////weaponsList[_weaponIndex].weaponGameObject.SetActive(true);
        ////weaponsList[_weaponIndex].weaponLocation.transform.position = Vector3.zero;
        ////weaponsList[_weaponIndex].weaponLocation.transform.rotation = Quaternion.identity;
        //weaponsList[_weaponIndex].weaponGameObject.transform.SetParent(weaponsList[_weaponIndex].weaponLocation);
        //weaponsList[_weaponIndex].weaponGameObject.transform.localPosition = Vector3.zero;
        //weaponsList[_weaponIndex].weaponGameObject.transform.localRotation = Quaternion.identity;



        //weaponIndex = _weaponIndex;
        //AttachAvailableWeapons(weaponIndex);
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new AttachWeaponComponent
            {
                attachedWeaponSlot = 0,//-1 is buggy but needed for pickup weapon IK
                attachWeaponType = (int)primaryWeapon.weaponType,
                attachSecondaryWeaponType = (int)secondaryWeapon.weaponType,
                weaponsAvailableCount = weaponsList.Count
            });
        e = entity;
        manager = dstManager;

    }




}


public class AttachWeaponSystem : SystemBase
{
    protected override void  OnUpdate()
    {


        var bufferFromEntity = GetBufferFromEntity<WeaponItemComponent>();


        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                //ref WeaponItemComponent AttachWeaponComponent,
                in Entity e,
                in  WeaponManager attachWeapon,
                in PlayerComponent player
            ) =>
            {

                //var AttachWeaponComponent = bufferFromEntity[e];


                //int currentSlot = AttachWeaponComponent.attachedWeaponSlot;
                //int weaponCount = attachWeapon.weaponsList.Count;
                //if (inputController.leftBumperPressed && currentSlot >= 0)
                //if (currentSlot >= 0)
                //{
                //    for (int i = 0; i < weaponCount; i++)
                //    {
                //        Weapons weapon = attachWeapon.weaponsList[i];
                //        currentSlot++;
                //        if (currentSlot >= weaponCount) currentSlot = 0;
                //        if (weapon.isAttached == true)
                //        {
                //            attachWeapon.AttachPickWeapons(currentSlot);
                //            break;
                //        }

                //    }
                //}

            }
        ).Run();




        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (
                ref AttachWeaponComponent AttachWeaponComponent,
                in WeaponManager attachWeapon, in EnemyComponent enemyComponent
            ) =>
            {
                int currentSlot = AttachWeaponComponent.attachedWeaponSlot;
                int weaponCount = attachWeapon.weaponsList.Count;
                for (int i = 0; i < weaponCount; i++)
                {
                    Weapons weapon = attachWeapon.weaponsList[i];
                    currentSlot++;
                    if (currentSlot >= weaponCount) currentSlot = 0;
                    if (weapon.isAttachedAtStart == true)
                    {
                        attachWeapon.AttachPickWeapons(currentSlot);
                        break;
                    }

                }

            }
        ).Run();



        //return default;
    }

}

