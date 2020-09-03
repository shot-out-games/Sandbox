using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;


public class GameObjectTracker : MonoBehaviour, IConvertGameObjectToEntity
{
    public EntityManager manager;
    public Entity e;
    Vector3 scenePosition;


    void OnEnable()
    {

    }
    void Awake()
    {

    }

    private void LateUpdate()
    {
        scenePosition = transform.position;


        if (e == Entity.Null) return;


        if (!manager.HasComponent(e, typeof(LocalToWorld))) return;




        Vector3 entityPosition = manager.GetComponentData<LocalToWorld>(e).Position;
        //entityPosition.y = 0;
        Quaternion entityRotation = manager.GetComponentData<LocalToWorld>(e).Rotation;
        transform.position = entityPosition;
        transform.rotation = entityRotation;


    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        manager = dstManager;
        e = entity;


        manager.SetComponentData<Translation>(e,
            new Translation
            {
                Value = scenePosition
            }
        ); ;

        //transform.position = scenePosition;


    }
}
