using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


struct FollowTriggerComponent : IComponentData
{
    //public Translation Translation;
    //public Rotation Rotation;
    //public Scale Scale;
    public float value;


}



public class FollowTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    [SerializeField] private Transform trigger;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new Translation { Value = trigger.position }
        );

        dstManager.AddComponentData(entity,
            new Rotation { Value = trigger.rotation }
        );

        dstManager.AddComponentData(entity,
            new FollowTriggerComponent()
        );






    }
}
