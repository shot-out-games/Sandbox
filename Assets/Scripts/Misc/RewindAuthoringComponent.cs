using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct RewindComponent : IComponentData
{
    public bool on;
    public bool pressed;
}

public class RewindAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{


    public bool on;
    
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RewindComponent() {on = on});



    }
}
