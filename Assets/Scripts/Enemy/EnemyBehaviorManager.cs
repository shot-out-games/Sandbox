using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct EnemyWeaponMovementComponent : IComponentData, IMovement
{
    public float speedMultiple;
    public float speed;
    public float combatStrikeDistanceZoneBegin;
    public float combatStrikeDistanceZoneEnd;
    public float chaseRange;
    public Vector3 originalPosition;
    public bool enabled;//true if currently active movement state

    //change to enemy behavior component ??
    public bool useDistanceFromStation;
    public bool switchUp; //if true enemy will change states when tracking
    public float switchUpTimer;

    public float originalSwitchUpTime;
    public float currentSwitchUpTime;



}

public struct EnemyMeleeMovementComponent : IComponentData
{
    public float speed;
    public float combatStrikeDistanceZoneBegin;
    public float combatStrikeDistanceZoneEnd;
    public float combatRangeDistance;
    public float chaseRange;
    public float aggression;
    public float maxHealth;
    public float speedMultiple;
    public bool backup;
    public Vector3 originalPosition;
    public bool enabled;//true if currently active movement state

    //change to enemy behavior component ??
    public bool useDistanceFromStation;
    public bool switchUp; //if true enemy will change states when tracking
    public float switchUpTimer;

    public float originalSwitchUpTime;
    public float currentSwitchUpTime;



}


public struct EnemyMovementComponent : IComponentData
{
    public float speedMultiple;
    public float speed;
    public float combatStrikeDistanceZoneBegin;
    public float combatStrikeDistanceZoneEnd;
    public float combatRangeDistance;
    public float chaseRange;
    public float aggression;
    public float maxHealth;
    public bool backup;
    public Vector3 originalPosition;
    public bool enabled;//true if currently active movement state


    //change to enemy behavior component ??
    public bool useDistanceFromStation; //if true distance from original station is used to decide chase or not
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
        dstManager.AddComponentData(entity, new EnemyMovementComponent { speedMultiple = 1.0f });
        dstManager.AddComponentData(entity,
            new EnemyMovementComponent
            {
                speedMultiple = 1.0f,
                combatStrikeDistanceZoneBegin = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneBegin,
                combatStrikeDistanceZoneEnd = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneEnd,
                useDistanceFromStation = useDistanceFromStation,
                originalPosition = transform.position,
                switchUp = switchUp,
                originalSwitchUpTime = switchUpTime,
                currentSwitchUpTime = switchUpTime,
                enabled = basicMovement
            }
        );

        dstManager.AddComponentData(entity, new EnemyMeleeMovementComponent
        {
            speedMultiple = 1.0f,
            combatStrikeDistanceZoneBegin = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneBegin,
            combatStrikeDistanceZoneEnd = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneEnd,
            useDistanceFromStation = useDistanceFromStation,
            originalPosition = transform.position,
            switchUp = switchUp,
            originalSwitchUpTime = switchUpTime,
            currentSwitchUpTime = switchUpTime,
            enabled = meleeMovement

        });
        dstManager.AddComponentData(entity,
            new EnemyWeaponMovementComponent
            {
                speedMultiple = 1.0f,
                combatStrikeDistanceZoneBegin = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneBegin,
                combatStrikeDistanceZoneEnd = GetComponent<EnemyRatings>().Ratings.combatStrikeDistanceZoneEnd,
                useDistanceFromStation = useDistanceFromStation,
                originalPosition = transform.position,
                switchUp = switchUp,
                originalSwitchUpTime = switchUpTime,
                currentSwitchUpTime = switchUpTime,
                enabled = weaponMovement
            }
        );

    }
}
