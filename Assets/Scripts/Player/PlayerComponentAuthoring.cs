using UnityEngine;
using Unity.Entities;
using Unity.Transforms;


[System.Serializable]
public struct StatsComponent : IComponentData
{
    public int shotsFired;
    public int shotsLanded;

}

[System.Serializable]
public struct SkillTreeComponent : IComponentData
{

    [System.NonSerialized]
    public Entity e;
    public int CurrentLevel;
    public int CurrentLevelXp;
    public int PointsNextLevel;

    public int availablePoints;
    public int SpeedPts;
    public int PowerPts;
    public int ChinPts;

    public float baseSpeed;


}




[RequiresEntityConversion]


public class PlayerComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity playerEntity;
    public EntityManager manager;

    [SerializeField]
    private bool checkWinCondition = true;

    [SerializeField]
    private bool checkLossCondition = true;




    //void LateUpdate()
    //{
    //    if (manager == null) return;
    //    if (!manager.HasComponent(playerEntity, typeof(Translation))) return;
    //    manager.SetComponentData(playerEntity, new Translation { Value = transform.position });
    //}


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        playerEntity = entity;
        manager = dstManager;

        dstManager.AddComponent<PlayerComponent>(entity);
        dstManager.AddComponentData(entity, new Pause { value = 0 });
        dstManager.AddComponentData(entity,
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

        dstManager.AddComponentData(entity,
            new LevelCompleteComponent
            {
                active = true,
                goalCounter = 0,
                goalCounterTarget = 0,//ie how many players you have to save - usually zero
                targetReached = false,
                checkWinCondition = checkWinCondition
            }
        );

        dstManager.AddComponentData(entity, new DeadComponent
        {
            tag = 1,
            isDead = false,
            checkLossCondition = checkLossCondition

        }
        );

        dstManager.AddComponentData(entity, new SkillTreeComponent()
        {
            e = entity,
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

        dstManager.AddComponentData(entity, new StatsComponent()
            {
                shotsFired = 0,
                shotsLanded = 0
            }
        );


    }


}
