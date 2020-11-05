using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


public enum ShowText3D
{
    none,
    hitDamage,
    hitScore
}

[System.Serializable]
public struct HealthComponent : IComponentData
{
    public float TotalDamageLanded;
    public float TotalDamageReceived;
    public bool AlwaysDamage;//ignore hit weights and similar
    public float ShowDamageMin;
    public bool ShowDamage;
    public ShowText3D ShowText3D;
    //Entity Entity;
}



public struct DamageComponent : IComponentData
{
    public float DamageLanded;
    public float DamageReceived;
    public float StunLanded;

}


public class HealthBar : MonoBehaviour, IConvertGameObjectToEntity
{

    public Image _healthBar = null;
    public Entity entity;
    private EntityManager entityManager;
    [SerializeField] private bool alwaysDamage;

    public TextMeshPro score3dText;
    private TextMeshPro score3dTextInstance;


    [SerializeField] private float showDamageMin = 50;
    [SerializeField] private ShowText3D showText3D = ShowText3D.hitDamage;




    void Start()
    {
        if (entity == Entity.Null) return;
        if (score3dText)
        {
            var ps = Instantiate(score3dText);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            score3dTextInstance = ps;
        }



        HealthChange();
    }






    public void ShowText3dValue(int value)
    {
        score3dTextInstance.text = value.ToString();
        Debug.Log("val " + value);

    }



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


  

    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager = dstManager;
        entity = _entity;
        entityManager.AddComponentData(entity, new HealthComponent
        {
            TotalDamageLanded = 0, TotalDamageReceived = 0,
            AlwaysDamage = alwaysDamage,
            ShowDamageMin = showDamageMin,
            ShowText3D = showText3D
        });
    }
}
