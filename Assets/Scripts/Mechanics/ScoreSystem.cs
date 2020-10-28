using SandBox.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//[UpdateAfter(typeof(FlingMechanicSystem))]

public class ScoreSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((ref ScoreComponent score, in ScoreComponentAuthoring scoreComponentAuthoring) =>
            {
                if (score.pointsScored == true)
                {
                    score.score = score.score + score.defaultPointsScored;
                    score.pointsScored = false;
                }

                scoreComponentAuthoring.ShowLabelScore(score.score);
            }
        ).Run();




    }
}
