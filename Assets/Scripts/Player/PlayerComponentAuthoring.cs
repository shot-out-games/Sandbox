using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
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
    public float CurrentLevelXp;
    public int PointsNextLevel;

    public int availablePoints;
    public int SpeedPts;
    public int PowerPts;
    public int ChinPts;
    public float TotalPoints;

    public float baseSpeed;


}

[System.Serializable]
public struct PlayerComponent : IComponentData
{
    public int index;//1 is p1 2 is p2 etc 1 is required for skill tree group
    public int keys;
    public int tag;
    public float speed;
    public float power;
    public float maxHealth;
    public bool threeD;
    public float3 startPosition;

}




public class PlayerComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity playerEntity;
    public EntityManager manager;
    private Animator animator;

    [SerializeField]
    int player_index = 0;

    [SerializeField]
    private bool checkWinCondition = true;

    [SerializeField]
    private bool checkLossCondition = true;


    public bool threeD;
    public int skillTreePointsToNextLevel = 10;

    [SerializeField]
    bool paused = false;

    //void LateUpdate()
    //{
    //    if (manager == null) return;
    //    if (!manager.HasComponent(playerEntity, typeof(Translation))) return;
    //    manager.SetComponentData(playerEntity, new Translation { Value = transform.position });
    //}

    void Awake()
    {
        //Debug.Log("player awake ");
        animator = GetComponent<Animator>();

    }

    void Start()
    {
        //Debug.Log("player start ");

    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        playerEntity = entity;
        manager = dstManager;

        conversionSystem.AddHybridComponent(animator);

        dstManager.AddComponentData(entity, new PlayerComponent
        {

            index = player_index,
            threeD = threeD,
            startPosition = manager.GetComponentData<Translation>(entity).Value
        }

            );


        if (paused == true)
        {
            dstManager.AddComponent<Pause>(entity);
        }


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
            TotalPoints = 0,
            SpeedPts = 0,
            PowerPts = 0,
            ChinPts = 0,
            baseSpeed = 0,
            CurrentLevel = 1,
            CurrentLevelXp = 0,
            PointsNextLevel = skillTreePointsToNextLevel

        }

        );

        dstManager.AddComponentData(entity, new StatsComponent()
        {
            shotsFired = 0,
            shotsLanded = 0
        }
        );

        dstManager.AddComponentData(entity, new CheckedComponent());


        //Debug.Log("player convert ");



    }


}
