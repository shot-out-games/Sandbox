using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



[System.Serializable]

public struct ScoreComponent : IComponentData
{
    public int score;
    public int rank;
    public bool pointsScored;
    public int lastPointValue;
    public int defaultPointsScored;
    public int scoreChecked;
    public float timeSinceLastScore;
    public int streak;
    public int combo;
    public Entity scoredAgainstEntity;


}


public class ScoreComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{
    [SerializeField]
    private TextMeshProUGUI labelScore;
    [SerializeField]
    private TextMeshProUGUI labelStreak;
    [SerializeField]
    private TextMeshProUGUI labelCombo;
    [SerializeField]
    private TextMeshProUGUI labelLevel;

    [SerializeField] private int defaultPointsScored = 100;

    public void ShowLabelStreak(int streak)
    {
        labelStreak.text = streak.ToString();
    }


    public void ShowLabelScore(int score)
    {
        labelScore.text = score.ToString();
    }

    public void ShowLabelCombo(int combo)
    {
        labelCombo.text = combo.ToString();
    }

    public void ShowLabelLevel(int level)
    {
        labelLevel.text= level.ToString();
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
