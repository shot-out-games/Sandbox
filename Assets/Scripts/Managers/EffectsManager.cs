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
}



public class EffectsManager : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private float timeBetween = .1f;
    [SerializeField]
    private float timeActive = .5f;

    [SerializeField] private bool pauseEffect;
    public ParticleSystem powerTriggerEffect;
    public AudioClip powerTriggerAudioClip;

    public ParticleSystem powerEnabledEffect;
    public AudioClip powerEnabledAudioClip;

    public ParticleSystem playerDeadEffect;
    public ParticleSystem playerDeadEffectInstance;
    public AudioClip playerDeadAudioClip;

    public ParticleSystem playerHurtEffect;
    public AudioClip playerHurtAudioClip;


    public AudioClip playerLevelCompleteClip;
    public AudioSource audioSource;


    void Start()
    {
        if (playerHurtEffect)
        {
            playerHurtEffect.transform.SetParent(transform);
            playerHurtEffect.transform.localPosition = new Vector3(0, playerHurtEffect.transform.localPosition.y, 0);
        }

        if (playerDeadEffect)
        {
            var ps = Instantiate(playerDeadEffect);
            ps.transform.parent = transform;
            ps.transform.localPosition = new Vector3(0, ps.transform.localPosition.y, 0);
            playerDeadEffectInstance = ps;
        }


        //if (playerDeadEffect)
        //{
        //    playerDeadEffect.transform.SetParent(transform);
        //    playerDeadEffect.transform.localPosition = new Vector3(0, playerDeadEffect.transform.localPosition.y, 0);
        //}

        //if (powerTriggerEffect)
        //{
        //    powerTriggerEffect.transform.SetParent(transform);
        //    powerTriggerEffect.transform.localPosition = new Vector3(0, powerTriggerEffect.transform.localPosition.y, 0);
        //}

        //if (powerEnabledEffect)
        //{
        //    powerEnabledEffect.transform.SetParent(transform);
        //    powerEnabledEffect.transform.localPosition = new Vector3(0, powerEnabledEffect.transform.localPosition.y, 0);

        //}

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {

        //for (int i = 0; i < NpcGameObjects.Length; i++)
        //{
        // Debug.Log("npc " + NpcGameObjects[i]);
        //referencedPrefabs.Add(playerDeadEffect);
        //}
    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EffectsComponent { timeBetween = timeBetween, timeActive = timeActive, pauseEffect = pauseEffect});
    }
}
