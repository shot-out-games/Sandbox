using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;


public struct ItemComponent : IComponentData
{
    public bool activator;
    public int index;
    //public Entity TopEntity;
}



public class ItemComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
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
    //    public float scale;
    [SerializeField] public bool activator;
    private EntityManager manager;
    private Entity e;
    //public GameObject TopPrefab;
    public ParticleSystem ps;
    public int index;


    void Awake()
    {
        if (ps != null)
            ps.Play(true);

    }
    public void DrawTransparent()
    {

        //Debug.Log("hole " + open);
    }



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        dstManager.AddComponentData(entity, new ItemComponent()
        {
            activator = activator,
            index = index

        });
        manager = dstManager;
        e = entity;


    }
}
