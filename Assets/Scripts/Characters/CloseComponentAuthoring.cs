using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[Serializable]

public struct CloseComponent : IComponentData
{
    public bool active;
    public float maxDistance;
    public bool isDamaging;
    public bool playEffects;

}



public class CloseComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //public bool matchupClosest = true;
    //public bool leader = false;
    [SerializeField]
    private float maxDistance = 1;
    [SerializeField] bool canFreeze;
    [SerializeField] bool isDamaging;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
        //
        // * You can add more than one component to the entity. It's also OK to not add any at all.
        //
        // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
        //   to do it, instead of adding entities through 'dstManager' directly.
        //
        // For example,
         dstManager.AddComponentData(entity, new CloseComponent {maxDistance = maxDistance, isDamaging = isDamaging});

        if (canFreeze)
        {
            dstManager.AddComponentData(entity, new FreezeComponent() );
        }



    }
}
