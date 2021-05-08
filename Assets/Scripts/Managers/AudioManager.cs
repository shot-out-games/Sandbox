using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AudioManager : MonoBehaviour, IConvertGameObjectToEntity
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<AudioSourceComponent>(entity);
    }
}
