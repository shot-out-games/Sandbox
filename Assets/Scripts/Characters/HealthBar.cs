using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct HealthComponent : IComponentData
{
    public float TotalDamageLanded;
    public float TotalDamageReceived;
    [System.NonSerialized]
    Entity Entity;
}



public struct DamageComponent : IComponentData
{
    public float DamageLanded;
    public float DamageReceived;
}


public class HealthBar : MonoBehaviour, IConvertGameObjectToEntity
{

    public Image _healthBar = null;
    public Entity entity;
    private EntityManager entityManager;
    public void HealthChange()
    {

        if (_healthBar == null || !entityManager.HasComponent<HealthComponent>(entity))
        {
            return;
        }

        float maxHealth = 100f;

        if (entityManager.HasComponent<RatingsComponent>(entity))
        {
            maxHealth = entityManager.GetComponentData<RatingsComponent>(entity).maxHealth;
        }

        float damage = entityManager.GetComponentData<HealthComponent>(entity).TotalDamageReceived;

        var pct = 1.00f - (damage / maxHealth);
        if (pct < 0)
        {
            pct = 0;
        }

        _healthBar.gameObject.transform.localScale = new Vector3(pct, 1f, 1f);

    }



    void Update()
    {
        if (entity == Entity.Null) return;
        HealthChange();
    }


    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager = dstManager;
        entity = _entity;
        entityManager.AddComponentData(entity, new HealthComponent { TotalDamageLanded = 0, TotalDamageReceived = 0 });
    }
}
