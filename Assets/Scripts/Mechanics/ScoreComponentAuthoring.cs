using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;




public struct ScoreComponent : IComponentData
{
    public int score;
    public int rank;
    public bool pointsScored;
    public int defaultPointsScored;
    public int scoreChecked;
    public float timeSinceLastScore;
    public int streak;
    public int combo;


}


public class ScoreComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity

{
    [SerializeField]
    private TextMeshProUGUI labelScore;
    [SerializeField]
    private TextMeshProUGUI labelStreak;
    [SerializeField]
    private TextMeshProUGUI labelCombo;

    [SerializeField] private int defaultPointsScored = 100;

    public void ShowLabelStreak(int streak)
    {
        labelStreak.text = "Streak " + streak.ToString();
    }


    public void ShowLabelScore(int score)
    {
        labelScore.text = "Score : " + score;
    }

    public void ShowLabelCombo(int score)
    {
        labelCombo.text = "Combo: " + score;
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
