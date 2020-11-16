using Rewired;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct CubeComponent : IComponentData
{
    public bool dead;

}

public class CubeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    private Entity e;

    void Start()
    {

    }

     void Update()
    {
    

    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
           dstManager.AddComponentData(entity, new CubeComponent());
           manager = dstManager;
           e = entity;


    }
}
