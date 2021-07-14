﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;



public struct EnemyBehaviourComponent : IComponentData
{
    public float speedMultiple;
    public float speed;
    public bool breakRoute;
    public bool useDistanceFromStation;
    public float chaseRange;
    public float aggression;
    public float maxHealth;


}
public struct EnemyWeaponMovementComponent : IComponentData, IMovement
{
    //public float speedMultiple;
    public Vector3 originalPosition;
    public bool enabled;//true if currently active movement state

    //change to enemy behavior component ??
    public bool switchUp; //if true enemy will change states when tracking
    public float switchUpTimer;

    public float originalSwitchUpTime;
    public float currentSwitchUpTime;

    public float shootRangeDistance;

}

public struct EnemyMeleeMovementComponent : IComponentData
{
    public float combatStrikeDistanceZoneBegin;
    public float combatStrikeDistanceZoneEnd;
    public float combatRangeDistance;
    //public float aggression;
    //public float maxHealth;
    //public float speedMultiple;
    public bool backup;
    public Vector3 originalPosition;
    public bool enabled;//true if currently active movement state

    //change to enemy behavior component ??
    public bool switchUp; //if true enemy will change states when tracking
    public float switchUpTimer;

    public float originalSwitchUpTime;
    public float currentSwitchUpTime;



}




public struct EnemyMovementComponent : IComponentData
{
    public bool backup;
    public Vector3 originalPosition;
    public bool enabled;//true if currently active movement state


    //change to enemy behavior component ??
    //public bool useDistanceFromStation; //if true distance from original station is used to decide chase or not
    public bool switchUp; //if true enemy will change states when tracking
    public float switchUpTimer;

    public float originalSwitchUpTime;
    public float currentSwitchUpTime;



}




public class EnemyBehaviorManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public NavigationStates navigationStates;
    public bool useDistanceFromStation; //if true distance from original station is used to decide chase or not
    public bool switchUp; //if true enemy will change states when tracking
    public float switchUpTime = 6.0f;
    [SerializeField] bool canFreeze;
    [SerializeField]
    bool breakRoute = true;

    //public float chaseRange;
    //public float combatRangeDistance;
    //public float shootRangeDistance;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        bool basicMovement = false;
        bool meleeMovement = false;
        bool weaponMovement = false;


        if (navigationStates == NavigationStates.Movement)
        {
            basicMovement = true;
        }
        else if (navigationStates == NavigationStates.Melee)
        {
            meleeMovement = true;
        }
        else if (navigationStates == NavigationStates.Weapon)
        {
            weaponMovement = true;
        }
        dstManager.AddComponentData(entity, new EnemyBehaviourComponent
        {
            breakRoute = breakRoute,
            useDistanceFromStation = useDistanceFromStation,
            chaseRange = GetComponent<EnemyRatings>().Ratings.chaseRange,
            speedMultiple = 1.0f


        });

        //dstManager.AddComponentData(entity, new EnemyMovementComponent { speedMultiple = 1.0f });
        dstManager.AddComponentData(entity,
            new EnemyMovementComponent
            {
                originalPosition = transform.position,
                switchUp = switchUp,
                originalSwitchUpTime = switchUpTime,
                currentSwitchUpTime = switchUpTime,
                enabled = basicMovement
            }
        );

        dstManager.AddComponentData(entity, new EnemyMeleeMovementComponent
        {
            combatStrikeDistanceZoneBegin = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneBegin,
            combatStrikeDistanceZoneEnd = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneEnd,
            originalPosition = transform.position,
            switchUp = switchUp,
            originalSwitchUpTime = switchUpTime,
            currentSwitchUpTime = switchUpTime,
            combatRangeDistance = GetComponent<EnemyRatings>().Ratings.combatRangeDistance,
            enabled = meleeMovement

        });
        dstManager.AddComponentData(entity,
            new EnemyWeaponMovementComponent
            {
                originalPosition = transform.position,
                switchUp = switchUp,
                originalSwitchUpTime = switchUpTime,
                currentSwitchUpTime = switchUpTime,
                shootRangeDistance = GetComponent<EnemyRatings>().Ratings.shootRangeDistance,
                enabled = weaponMovement
            }
        );

        if (canFreeze)
        {
            dstManager.AddComponentData(entity, new FreezeComponent());
        }

    }
}
