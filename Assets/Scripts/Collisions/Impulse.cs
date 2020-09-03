using Cinemachine;
using Unity.Entities;
using UnityEngine;

public class Impulse : MonoBehaviour, IConvertGameObjectToEntity
{

    public CinemachineImpulseSource impulseSource;
    private Entity e;
    private EntityManager manager;



    void Awake()
    {

        impulseSource = GetComponent<CinemachineImpulseSource>();

    }




    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        conversionSystem.AddHybridComponent(impulseSource);

    }
}
