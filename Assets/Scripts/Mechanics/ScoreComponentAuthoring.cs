using System;
using Rewired.Utils.Libraries.TinyJson;
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
    public int addBonus;//used for bank shot bonus for example


    public int scoreChecked;
    public float timeSinceLastScore;
    public int streak;
    public int combo;
    public bool lastShotConnected;

    public Entity scoringAmmoEntity;

    [NonSerialized]
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
        if (labelStreak == null) return;
        labelStreak.text = streak.ToString();
    }


    public void ShowLabelScore(int score)
    {

        if (labelScore == null) return;

        labelScore.text = score.ToString();
        
    }

    public void ShowLabelCombo(int combo)
    {
        if (labelCombo == null) return;
        labelCombo.text = combo.ToString();
    }

    public void ShowLabelLevel(int level)
    {
        if (labelLevel == null) return;
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
