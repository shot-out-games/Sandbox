using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class NpcBehaviorManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public NavigationStates navigationStates;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (navigationStates == NavigationStates.Movement)
        {
            dstManager.AddComponentData(entity, new NpcMovementComponent { speedMultiple = 1.0f });
            dstManager.AddComponentData(entity,
                new NpcMovementComponent
                {
                    speedMultiple = 1.0f,
                    combatStrikeDistanceZoneBegin = GetComponent<NpcRatings>().Ratings.combatStrikeDistanceZoneBegin,
                    combatStrikeDistanceZoneEnd = GetComponent<NpcRatings>().Ratings.combatStrikeDistanceZoneEnd
                }
                );

        }
        else if (navigationStates == NavigationStates.Melee)
        {
            //to do
        }
        else if (navigationStates == NavigationStates.Weapon)
        {
            //to do
        }

    }
}
