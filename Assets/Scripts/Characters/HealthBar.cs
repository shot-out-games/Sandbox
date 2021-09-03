using System.Collections;
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
    public bool combineDamage;
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
    public float ScorePointsReceived;//to track if hit and points scored by player how many and what enemy
    public float StunLanded;

}


public class HealthBar : MonoBehaviour, IConvertGameObjectToEntity
{

    public Image _healthBar = null;
    bool showHealth = true;
    public Entity entity;
    private EntityManager entityManager;
    [SerializeField] private bool alwaysDamaging;

    public TextMeshPro score3dText;
    private TextMeshPro score3dTextInstance;

    [SerializeField] bool combineDamage = false;
    [SerializeField] private float showDamageMin = 50;
    [SerializeField] private ShowText3D showText3D = ShowText3D.hitDamage;
    [SerializeField] float showTime = 3;
    float alphaTime = 0;


    //MeshRenderer renderer;
    //Material material;


    void Start()
    {
        if (score3dText)
        {
            var ps = Instantiate(score3dText);
            ps.transform.SetParent( transform, false);
            ps.transform.localPosition = new Vector3(0, 5, 0);
            score3dTextInstance = ps;
            //renderer = score3dTextInstance.GetComponent<MeshRenderer>();
            //material = renderer.sharedMaterial;
            SetAlpha(0);
        }
       // if (entity == Entity.Null) return;
        //HealthChange();


    }


    void SetAlpha(float alphaValue)
    {
        Color color = score3dTextInstance.color;
        color.a = alphaValue;
        score3dTextInstance.color = color;
    }


    void Update()
    {
        if (showHealth == true && entity != Entity.Null)
        {
            showHealth = false;
            HealthChange();
        }


        if (alphaTime > 0)
        {
            alphaTime += Time.deltaTime;
            if(alphaTime > showTime)
            {
                alphaTime = 0;
                SetAlpha(0);
            }
            else
            {
                SetAlpha((showTime - alphaTime) / showTime);
            }
        }
    }


    public void ShowText3dValue(int value)
    {
        score3dTextInstance.text = value.ToString();
        //Debug.Log("val " + value);
        SetAlpha(1);
        alphaTime += Time.deltaTime; 
        //StartCoroutine(Wait(showTime));
        //SetAlpha(1);


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



    //IEnumerator Wait(float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    SetAlpha(0);
    //    Debug.Log("val ");

    //}




    public void Convert(Entity _entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager = dstManager;
        entity = _entity;
        entityManager.AddComponentData(entity, new HealthComponent
        {
            combineDamage = combineDamage,
            TotalDamageLanded = 0, TotalDamageReceived = 0,
            AlwaysDamage = alwaysDamaging,
            ShowDamageMin = showDamageMin,
            ShowText3D = showText3D
        });
    }

}
