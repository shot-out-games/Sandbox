using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DebugScript : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform cam = Camera.main.transform;

        transform.position = transform.InverseTransformDirection(cam.forward);

        
    }
}
