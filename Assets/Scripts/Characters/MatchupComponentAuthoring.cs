using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[Serializable]
public struct MatchupComponent : IComponentData
{
    public bool matchupClosest;
    public bool leader;
    public float AngleRadians;
    public float ViewDistanceSQ;
    public bool View360;
}

//public struct CloseComponent : IComponentData
//{
//    public bool active;


//}



public class MatchupComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
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
    public bool matchupClosest = true;
    public bool leader = false;

    public float AngleRadians = 180;
    public float ViewDistanceSQ = 100;

    public bool View360 = false;

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
         dstManager.AddComponentData(entity, 
             new MatchupComponent
             {
                 matchupClosest = matchupClosest, leader = leader, AngleRadians = AngleRadians, ViewDistanceSQ = ViewDistanceSQ,
                 View360 = View360
             });
        
        
    }
}
