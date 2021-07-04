using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]


public struct RatingsComponent : IComponentData
{
    public int tag;
    public float speed;
    public float hitPower;
    public float maxHealth;
    public float shootRangeDistance;
    public float chaseRangeDistance;
    public float combatRangeDistance;

    public float gameSpeed;
    public float gameWeaponPower;
    public float WeaponPower;


}


