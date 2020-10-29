using SandBox.Player;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//[UpdateBefore(typeof(BasicWinnerSystem))]

public class ScoreSystem : SystemBase
{
    protected override void OnUpdate()
    {
        int currentScore = 0;
        int scoreChecked = 0;
        Entities.WithoutBurst().ForEach((ref ScoreComponent score, in ScoreComponentAuthoring scoreComponentAuthoring) =>
            {
                if (score.pointsScored == true)
                {
                    score.score = score.score + score.defaultPointsScored;
                    score.pointsScored = false;
                    scoreComponentAuthoring.ShowLabelScore(score.score);
                }
                scoreChecked = score.scoreChecked;
                currentScore = score.score;
            }
        ).Run();

        //run rank score loop only when endgame
        if (LevelManager.instance.endGame == true && scoreChecked == 0 || SaveManager.instance.updateScore == true)
        {
            SaveManager.instance.updateScore = false;
            int slot = SaveManager.instance.saveWorld.lastLoadedSlot - 1;
            List<float> scores = SaveManager.instance.saveData.saveGames[slot].scoreList;
            Debug.Log("cs " + currentScore);
            scores.Add(currentScore);
            scores.Sort();
            SaveManager.instance.saveData.saveGames[slot].scoreList = scores ;
            SaveManager.instance.SaveGameData();
            int rank = 1;
            int count = scores.Count;
            for (int i = 0; i < count; i++)
            {
                if (scores[i] > currentScore)
                {
                    rank += 1;
                }
            }

            Entities.WithoutBurst().ForEach
            (
                (ref ScoreComponent scoreComponent) =>
                {
                    scoreComponent.rank = rank;
                    scoreComponent.scoreChecked = 1;
                }
            ).Run();


            Entities.WithoutBurst().ForEach
            (
                (ref ScoresMenuComponent scoresMenuComponent) =>
                {
                    CalcScoreMenuLabels(scores, ref scoresMenuComponent);

                }
            ).Run();

        }
        //Entities.WithoutBurst().ForEach
        //(
        //    (ref ScoresMenuComponent scoresMenuComponent, in ScoreMenuGroup scoreMenuGroup) =>
        //    {

        //        if (showScores == true && scoresMenuComponent.index == 0)
        //        {
        //            scoresMenuComponent.index = 1;
        //            //CalcScores(scores, ref scoresMenuComponent);

        //            scoresMenuComponent.hide = false;
        //        }
        //    }


        //).Run();

    }



    void CalcScoreMenuLabels(System.Collections.Generic.List<float> scores, ref ScoresMenuComponent scoresMenuComponent)
    {
        int slot = scores.Count;
        if (scores.Count > 0)
        {
            scoresMenuComponent.hi1 = (int)scores[slot - 1];
        }

        if (scores.Count > 1)
        {
            scoresMenuComponent.hi2 = (int)scores[slot - 2];
        }

        if (scores.Count > 2)
        {
            scoresMenuComponent.hi3 = (int)scores[slot - 3];
        }

        if (scores.Count > 3)
        {
            scoresMenuComponent.hi4 = (int)scores[slot - 4];
        }

        if (scores.Count > 4)
        {
            scoresMenuComponent.hi5 = (int)scores[slot - 5];
        }



    }





















}

