using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;



public class ControlBar : MonoBehaviour, IConvertGameObjectToEntity
{

    public Image _controlBar = null;
    public Entity entity;
    private EntityManager entityManager;
    public void ControlChange()
    {

        if (_controlBar == null || !entityManager.HasComponent<ControlBarComponent>(entity))
        {
            return;
        }

        //float maxHealth = 100f;

        //if (entityManager.HasComponent<ControlBarComponent>(entity))
        //{
        //    maxHealth = entityManager.GetComponentData<ControlBarComponent>(entity).maxHealth;
        //}

        //maxHealth = 25f;

        float maxHealth = entityManager.GetComponentData<ControlBarComponent>(entity).maxHealth;



        float value = entityManager.GetComponentData<ControlBarComponent>(entity).value;

        var pct = value / maxHealth;
        if (pct < 0)
        {
            pct = 0;
        }

        _controlBar.gameObject.transform.localScale = new Vector3(pct, 1f, 1f);

    }



    void Update()
    {
        if (entity == Entity.Null) return;
        ControlChange();
    }


    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager = dstManager;
        entity = _entity;
        //entityManager.AddComponentData(entity, new HealthComponent { TotalDamageLanded = 0, TotalDamageReceived = 0 });
    }
}
