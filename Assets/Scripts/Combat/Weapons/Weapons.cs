﻿using System.Collections.Generic;
using UnityEngine;



public enum WeaponType
{
    None = 0,
    Gun = 1,
    Fireball = 2
}

[System.Serializable]

public class Weapons
{
    public bool isAttached;
    public WeaponType weaponType;
    public bool secondaryWeapon;
    public GameObject weaponGameObject;//actual weapon
    public Transform weaponLocation;//added transform to rig

}