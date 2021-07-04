using Unity.Entities;
using UnityEngine;

public class PlayerRatings : MonoBehaviour, IConvertGameObjectToEntity
{

    public PlayerRatingsScriptableObject Ratings;
    public float meleeWeaponPower = 1;
    public float hitPower = 10;//punch kick



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, 
            new RatingsComponent
            {
                tag = 1, maxHealth = Ratings.maxHealth, 
                speed = Ratings.speed,
                gameSpeed =  Ratings.speed,
                gameWeaponPower = meleeWeaponPower,
                WeaponPower = meleeWeaponPower,
                hitPower = hitPower
            })
            ;
    }


}
