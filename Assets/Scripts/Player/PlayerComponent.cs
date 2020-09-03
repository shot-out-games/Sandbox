using Unity.Entities;
[System.Serializable] 
public struct PlayerComponent : IComponentData
{
    public int index;
    public int  keys;
    public int tag;
    public float speed;
    public float power;
    public float maxHealth;

}




