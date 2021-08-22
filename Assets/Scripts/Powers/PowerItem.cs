using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;


[System.Serializable]
public struct PowerItemComponent : IComponentData
{
    public Entity pickedUpActor;
    public bool active;
    public bool enabled;
    public int powerType;
    public float speedTimeOn;
    public float speedTimeMultiplier;
    public float healthMultiplier;
    public Entity particleSystemEntity;

}





public class PowerItem : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public PowerType powerType;

    public bool alive = true;

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
    public AudioClip powerTriggerAudioClip;
    public AudioSource audioSource;

    public Mesh mesh;
    public Material material;
    public MeshRenderer meshRenderer;



    void Start()
    {
        if(audioSource) return;
        audioSource = GetComponent<AudioSource>();

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(powerEnabledEffectPrefab);
    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {


        conversionSystem.DeclareLinkedEntityGroup(this.gameObject);

        dstManager.AddComponent<AudioSourceComponent>(entity);


        e = entity;
        manager = dstManager;

        conversionSystem.AddHybridComponent(audioSource);
        conversionSystem.AddHybridComponent(this);




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

        dstManager.SetSharedComponentData(entity, new RenderMesh() { mesh = mesh, material = material });



    }


}
