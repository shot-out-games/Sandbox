using UnityEngine;
using Unity.Entities;


[RequiresEntityConversion]

public class NpcRatings : MonoBehaviour, IConvertGameObjectToEntity
{

    public NpcRatingsScriptableObject Ratings;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RatingsComponent { tag = 3, maxHealth = Ratings.maxHealth, speed = Ratings.speed });//npc
    }



}
