using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public struct SaveComponent : IComponentData
{
    public bool value;
    public bool saveGame;
    public bool saveScore;
}




    public class SaveAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
        public bool saveGame;
        public bool saveScore;

    
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
           //dstManager.AddComponentData(entity, new SaveComponent() { saveGame = saveGame, saveScore = saveScore});
        
        
    }
}
