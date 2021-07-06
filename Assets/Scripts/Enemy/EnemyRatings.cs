using UnityEngine;
using Unity.Entities;



public class EnemyRatings : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] bool randomize;
    //[SerializeField] float hitPower = 10;
    public EnemyRatingsScriptableObject Ratings;
    EntityManager manager;
    Entity e;

    void Generate()
    {
        float multiplier = .5f;
        RatingsComponent ratings = manager.GetComponentData<RatingsComponent>(e);
        ratings.speed = Random.Range(ratings.speed * multiplier, ratings.speed * (2 - multiplier)) ;
        ratings.shootRangeDistance = Random.Range(ratings.shootRangeDistance * multiplier, ratings.shootRangeDistance * (2 - multiplier));
        ratings.chaseRangeDistance = Random.Range(ratings.chaseRangeDistance * multiplier, ratings.chaseRangeDistance * (2 - multiplier));



        manager.SetComponentData<RatingsComponent>(e, ratings);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {


        dstManager.AddComponentData(
            entity,
            new RatingsComponent
            {
                tag = 2, maxHealth = Ratings.maxHealth,
                speed = Ratings.speed,
                shootRangeDistance = Ratings.shootRangeDistance,
                chaseRangeDistance = Ratings.chaseRange,
                combatRangeDistance = Ratings.combatRangeDistance,
                hitPower = Ratings.hitPower
            });

        e = entity;
        manager = dstManager;

        if (randomize == true)
        {
            Generate();
        }



    }



}
