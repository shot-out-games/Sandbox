using UnityEngine;
using Unity.Entities;



public class EnemyRatings : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] bool randomize;
    public EnemyRatingsScriptableObject Ratings;
    EntityManager manager;
    Entity e;

    void Generate()
    {
        float multiplier = .7f;
        RatingsComponent ratings = manager.GetComponentData<RatingsComponent>(e);
        ratings.speed = Random.Range(ratings.speed * multiplier, ratings.speed * (2 - multiplier));
        //ratings.shootRangeDistance = Random.Range(ratings.shootRangeDistance * multiplier, ratings.shootRangeDistance* (2 - multiplier));



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
                WeaponPower = 1,
                gameWeaponPower = 1
            });

        e = entity;
        manager = dstManager;

        if (randomize == true)
        {
            Generate();
        }



    }



}
