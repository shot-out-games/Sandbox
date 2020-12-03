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
    public bool primaryAttached = false;
    private bool attachedWeapon;

    //public CameraType weaponCamera;


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
        if(primaryWeapon.weaponGameObject == null) return;
        primaryAttached = true;
        var weaponInstance = Instantiate(primaryWeapon.weaponGameObject);
        weaponInstance.transform.SetParent(primaryWeapon.weaponLocation);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;


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

    }

    public void AttachSecondaryWeapon()
    {
        var weaponInstance = Instantiate(secondaryWeapon.weaponGameObject);
        weaponInstance.transform.SetParent(secondaryWeapon.weaponLocation);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;


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
                //weaponCamera = weaponCamera
            });
        e = entity;
        manager = dstManager;

    }




}
