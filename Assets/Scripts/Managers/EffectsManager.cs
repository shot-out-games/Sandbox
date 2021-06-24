using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Audio;


public struct EffectsComponent : IComponentData
{
    public bool pauseEffect;
    public bool soundPlaying;
    public bool playEffectAllowed;
    public EffectType playEffectType;
}



public class EffectsManager : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField] private bool pauseEffect;


    public ParticleSystem actorDeadEffectPrefab;
    [HideInInspector] public ParticleSystem actorDeadEffectInstance;
    public AudioClip actorDeadAudioClip;

    public ParticleSystem actorHurtEffectPrefab;
    [HideInInspector] public ParticleSystem actorHurtEffectInstance;
    public AudioClip actorHurtAudioClip;

    public ParticleSystem actorCloseEffectPrefab;
    [HideInInspector] public ParticleSystem actorCloseEffectInstance;
    public AudioClip actorCloseAudioClip;

    public AudioSource audioSource;
    
    void Start()
    {


        if (actorHurtEffectPrefab)
        {
            var ps = Instantiate(actorHurtEffectPrefab);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            actorHurtEffectInstance = ps;
        }

        if (actorDeadEffectPrefab)
        {
            var ps = Instantiate(actorDeadEffectPrefab);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            actorDeadEffectInstance = ps;
        }

        if (actorCloseEffectPrefab)
        {
            var ps = Instantiate(actorCloseEffectPrefab);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            actorCloseEffectInstance = ps;
        }



    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EffectsComponent { pauseEffect = pauseEffect});
    }
}
