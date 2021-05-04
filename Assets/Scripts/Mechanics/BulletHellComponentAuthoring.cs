using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BulletShooterComponent : IComponentData
{
    public bool active;
}


public class BulletHellComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool active = true;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BulletShooterComponent() { active = active });
    }
}
