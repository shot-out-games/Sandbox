using System.Numerics;
using Unity.Entities;
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
}





