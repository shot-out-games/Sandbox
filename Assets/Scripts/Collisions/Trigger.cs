using UnityEngine;
using Unity.Entities;

public class Trigger : MonoBehaviour, IConvertGameObjectToEntity
{

    public TriggerType Type;
    [SerializeField]
    private int index;

    public ParticleSystem triggerParticleSystem;
    [HideInInspector]
    public AudioSource triggerAudioSource;


    private void Start()
    {
        triggerAudioSource = GetComponent<AudioSource>();
    } 


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //parent needs to be fixed currently self
        TriggerComponent trigger = new TriggerComponent
        {
            Type = (int)Type,
            Entity = entity,
            ParentEntity = conversionSystem.GetPrimaryEntity(transform.gameObject),
            CurrentFrame = 0,
            index = index,
            Hit = false,
            Active = true
        };

        dstManager.AddComponentData(entity, trigger);
    }


}
