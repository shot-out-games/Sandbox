using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct NdeMechanicComponent : IComponentData
{
    public bool active;
    public float multiplier;

}


public class NdeComponentAuthoring: MonoBehaviour, IConvertGameObjectToEntity
{
    public bool active = true;
    public float multiplier = 1;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new NdeMechanicComponent {active = active, multiplier = multiplier});

    }
}
