using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


[System.Serializable]
public struct PowerItemComponent : IComponentData
{
    public Entity pickedUpActor;
    public bool active;
    public int powerType;
    public float speedTimeOn;
    public float speedTimeMultiplier;
    public float healthMultiplier;
    public Entity particleSystemEntity;

}





public class PowerItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public PowerType powerType;


    [SerializeField]
    bool active = true;
    public Entity e;
    EntityManager manager;

    [Header("Speed")]
    [SerializeField] private float speedTimeOn = 3.0f;
    [SerializeField] private float speedMultiplier = 3.0f;

    [Header("Health")]
    [SerializeField] private float healthMultiplier = .75f;

    public GameObject powerEnabledEffectPrefab;
    public GameObject powerEnabledEffectInstance;
    public AudioClip powerEnabledAudioClip;



    void Start()
    {
        //if (powerEnabledEffectPrefab)
        //{
        //    var ps = Instantiate(powerEnabledEffectPrefab);
        //    ps.transform.parent = transform;
        //    ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
        //    powerEnabledEffectInstance = ps;
        //    Debug.Log("ps");
        //}
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(powerEnabledEffectPrefab);
    }


    void Update()
    {
        //if (manager == default || e == Entity.Null) return;
        //if (manager.HasComponent<PowerItemComponent>(e) == false) return;

        //PowerItemComponent powerItem = manager.GetComponentData<PowerItemComponent>(e);
        //if (powerItem.active == false)
        //{
        //    gameObject.SetActive(false);
        //};

    }





    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        //conversionSystem.AddHybridComponent(powerEnabledEffectInstance);

        conversionSystem.DeclareLinkedEntityGroup(this.gameObject);

        e = entity;
        manager = dstManager;
        manager.AddComponentData<PowerItemComponent>(entity, new PowerItemComponent
        {
            particleSystemEntity = conversionSystem.GetPrimaryEntity(powerEnabledEffectPrefab),
            active = active,
            powerType = (int)powerType,
            speedTimeOn = speedTimeOn,
            speedTimeMultiplier = speedMultiplier,
            healthMultiplier = healthMultiplier
        });


        if (powerType == PowerType.Speed)
        {
            dstManager.AddComponentData(entity, new Speed
            {
                enabled = false,
                timer = 0,
                timeOn = 0,
                startTimer = false,
                originalSpeed = 0,
                multiplier = 0,
            }
           );

        }

        if (powerType == PowerType.Health)
        {
            dstManager.AddComponentData(entity, new HealthPower
            {
                enabled = false,
                healthMultiplier = 0
            }
            );
        }


        if (powerType == PowerType.Control)
        {

            dstManager.AddComponentData(entity, new ControlPower
            {
                enabled = false,
                controlMultiplier = 0
            }
        );

        }




    }


}
