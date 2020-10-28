using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using System;
using System.Collections.Generic;

[System.Serializable]

public struct NpcComponent : IComponentData
{
    public bool active;
    [System.NonSerialized]
    public Entity e;
    public int tag;
    public int index;
    public float speed;
    public float power;
    public float maxHealth;


}

//[RequiresEntityConversion]

//public class NpcComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
public class NpcComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity npcEntity;
    public EntityManager manager;

    [SerializeField]
    private bool checkWinCondition = true;

    [SerializeField]
    private bool checkLossCondition = true;

    public int spawnerIndex;
    void Awake()
    {
        //Debug.Log("spawned");
        //manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }




    void Start()
    {
        //Debug.Log("start");
        //        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        //     var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //npcEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;



    }


    void LateUpdate()
    {
        if (manager == default) return;
        if (!manager.HasComponent(npcEntity, typeof(Translation))) return;
        manager.SetComponentData(npcEntity, new Translation { Value = transform.position });
    }

    public void Spawned(string holeName)
    {
        //holeName = holeName.Replace("robot", "").Replace(" ", "");
        holeName = holeName.Replace("hole", "");
        spawnerIndex = Int32.Parse(holeName);
        //Debug.Log("npc saved id  " + holeName);
    }

    public void Activate()
    {
        var npcComponent = manager.GetComponentData<NpcComponent>(npcEntity);
        npcComponent.active = true;
        manager.SetComponentData<NpcComponent>(npcEntity, npcComponent);

          

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        npcEntity = entity;
        manager = dstManager;
        //Entity e = manager.Instantiate(npcEntity);

        AddComponents(npcEntity);


        //manager.Instantiate(npcEntity);
        //manager.Instantiate(npcEntity);
        //manager.Instantiate(npcEntity);

    }


    public void AddComponents(Entity npcEntity)
    {

        //Debug.Log("convert npc author " + npcEntity);

        manager.AddComponentData(npcEntity, new CharacterSaveComponent { saveIndex = spawnerIndex });


        manager.AddComponentData<NpcComponent>(npcEntity, new NpcComponent
            {
                active = false,
                e = npcEntity
            }
        );

        //manager.AddComponentData(npcEntity, new Pause { value = 0 });


        manager.AddComponentData(npcEntity,
            new WinnerComponent
            {
                active = true,
                goalCounter = 0,
                goalCounterTarget = 0,//ie how many players you have to save - usually zero
                targetReached = false,
                endGameReached = false,
                checkWinCondition = checkWinCondition
            }
        );

        manager.AddComponentData(npcEntity,
            new LevelCompleteComponent
            {
                active = true,
                goalCounter = 0,
                goalCounterTarget = 0,//ie how many players you have to save - usually zero
                targetReached = false,
                checkWinCondition = checkWinCondition
            }
        );


        manager.AddComponentData(npcEntity, new DeadComponent
            {
                tag = 3,
                isDead = false,
                checkLossCondition = checkLossCondition

            }
        );


        manager.AddComponentData(npcEntity, new SkillTreeComponent()
            {
                e = npcEntity,
                availablePoints = 0,
                SpeedPts = 0,
                PowerPts = 0,
                ChinPts = 0,
                baseSpeed = 0,
                CurrentLevel = 1,
                CurrentLevelXp = 0,
                PointsNextLevel = 10

            }

        );




    }

}


