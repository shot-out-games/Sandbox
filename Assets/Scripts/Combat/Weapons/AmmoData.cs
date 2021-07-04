using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct AmmoDataComponent : IComponentData
{

    public float AmmoTime;
    public float Strength;
    public float Damage;
    public float Rate;
    public float GameAmmoTime;
    public float GameStrength;
    public float GameDamage;
    public float GameRate;
    public bool ChargeRequired;

}

public class AmmoData : MonoBehaviour, IConvertGameObjectToEntity
{

    [Header("Ammo Ratings")]
    [SerializeField]
    bool randomize;
    [HideInInspector]
    public float AmmoTime;
    public float Strength;
    public float Damage;
    public float Rate;
    public bool ChargeRequired;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        //Debug.Log("convert ad ");

        dstManager.AddComponentData<AmmoDataComponent>(entity, new AmmoDataComponent()
            {
                Strength = Strength,
                Damage = Damage,
                Rate = Rate,
                AmmoTime = AmmoTime,
                GameStrength = Strength,
                GameDamage = Damage,
                GameRate = Rate,
                GameAmmoTime = AmmoTime,
                ChargeRequired = ChargeRequired
                
            }

        );



    }
}
