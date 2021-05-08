using System;
using System.Linq;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public struct EnemyComponent : IComponentData
{
    [System.NonSerialized]
    public Entity e;
    //public int saveIndex;
    public float3 position;
    public float speed;
    public float combatStrikeDistance;
    public float combatRangeDistance;
    public float chaseRange;
    public float aggression;
    public float maxHealth;
    public bool invincible;
    public float3 startPosition;
    public bool humanoid;

}



public class EnemyComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity enemyEntity;
    public EntityManager manager;

    [SerializeField] private bool humanoid = true;

    [SerializeField]
    private bool checkLossCondition = true;

    [SerializeField]
    bool checkWinCondition;

    [SerializeField]
    bool invincible;

    [SerializeField]
    int saveIndex;

    [SerializeField]
    bool paused = false;


    void LateUpdate()
    {
        if (manager == default) return;
        if (!manager.HasComponent(enemyEntity, typeof(Translation))) return;

        manager.SetComponentData(enemyEntity, new Translation { Value = transform.position });
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        enemyEntity = entity;
        manager = dstManager;
        dstManager.AddComponentData(entity,
            new EnemyComponent
            {
                e = entity,
                invincible = invincible,
                humanoid = humanoid,
                startPosition = manager.GetComponentData<Translation>(entity).Value
            }
            );

        if (paused == true)
        {
            dstManager.AddComponent<Pause>(entity);
        }


        dstManager.AddComponentData(entity, new StatsComponent()
        {
            shotsFired = 0,
            shotsLanded = 0
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




        dstManager.AddComponentData(entity, new WinnerComponent
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
            tag = 2,
            isDead = false,
            checkLossCondition = checkLossCondition

        }
        );

        //string str = gameObject.name;
        //str = str.Replace("enemy", "").Replace("(", "").Replace(")", "");


        string a = gameObject.name;
        string b = string.Empty;
        int val = 0;

        for (int i = 0; i < a.Length; i++)
        {
            if (Char.IsDigit(a[i]))
                b += a[i];
        }

        if (b.Length > 0)
            val = int.Parse(b);

        //int index = Int32.Parse(str);//fix
        int index = val;

        //Debug.Log("go " + a + " val " + index);

        dstManager.AddComponentData(entity, new CharacterSaveComponent { saveIndex = index });

        dstManager.AddComponentData(entity, new CheckedComponent());






    }


}

