using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



public enum SlashStates
{
    None,
    Started,
    InAction,
    Ended
    
}

public struct SlashComponent : IComponentData
{
    public bool slashActive;
    public int slashState;
    public float hkDamage;//for ld
    public bool animate;

}


public class SlashComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{
    public bool slashActive;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SlashComponent()
        {
            slashActive = slashActive,
            slashState = (int) SlashStates.None
            
        }

        );


    }
}
