using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;




public struct ScoreComponent : IComponentData
{
    public int score;
    public bool pointsScored;
    public int defaultPointsScored;


}


public class ScoreComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{
    [SerializeField]
    private TextMeshProUGUI labelScore;

    [SerializeField] private int defaultPointsScored = 100;



    public void ShowLabelScore(int score)
    {
        labelScore.text = "Score : " + score;
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ScoreComponent()
            {
                defaultPointsScored = defaultPointsScored
            }
        );




    }
}
