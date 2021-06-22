using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public struct EffectsComponent : IComponentData
{
    public bool pauseEffect;
    public float timeBetween;
    public float timeActive;
    public bool soundPlaying;
    public bool playEffect;
}



public class EffectsManager : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private float timeBetween = .1f;
    [SerializeField]
    private float timeActive = .5f;

    [SerializeField] private bool pauseEffect;


    public ParticleSystem actorDeadEffectPrefab;
    public ParticleSystem actorDeadEffectInstance;
    public AudioClip actorDeadAudioClip;

    public ParticleSystem actorHurtEffectPrefab;
    public ParticleSystem actorHurtEffectInstance;
    public AudioClip actorHurtAudioClip;

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



    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EffectsComponent { timeBetween = timeBetween, timeActive = timeActive, pauseEffect = pauseEffect});
    }
}
