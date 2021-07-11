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

        var flingGroup = GetComponentDataFromEntity<FlingMechanicComponent>(false);
        var damageGroup = GetComponentDataFromEntity<DamageComponent>(false);
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);


        int currentScore = 0;
        int scoreChecked = 0;
        Entities.WithoutBurst().ForEach((ref ScoreComponent score, in ScoreComponentAuthoring scoreComponentAuthoring, in Entity e,
            in InputController inputController
) =>
            {
                if (HasComponent<DamageComponent>(e))
                {
                    score.streak = 0;
                    //Debug.Log("streak 0");

                }


                if (score.pointsScored == true)
                {
                    if (HasComponent<FlingMechanicComponent>(e))
                    {
                        var fling = flingGroup[e];
                        fling.shotLanded = true;
                        score.combo += 1;
                        if (fling.resetTimerAfterHitLanded == true)
                        {
                            fling.inFlingTime = fling.inFlingMaxTime * .5f;
                        }

                        if (fling.lastShotConnected == true && score.combo == 1)//only add to streak  once per shot that is why we check combo count
                        {
                            score.streak += 1;
                        }
                        else
                        {
                            score.streak = 1;
                        }
                        fling.lastShotConnected = true;
                        flingGroup[e] = fling;
                        //Debug.Log("combo " + score.combo);

                    }
                    else
                    {



                        if(score.combo == 1) score.streak += 1;//add only when combo is 1
                        score.lastShotConnected = true;

                        //}


                    }



                    float defaultScore = score.defaultPointsScored;

                    float timeBonus = (5 - score.timeSinceLastScore) * defaultScore;
                    timeBonus = math.clamp(timeBonus, -.5f * defaultScore, 2f * defaultScore);

                    float streakBonus = math.pow(score.streak * defaultScore, 2) / 500;

                    float comboBonus = score.combo > 1 ? math.pow(score.combo * defaultScore ,2) / 200 : 0;
                    //Debug.Log("combo Bonus " + comboBonus);

                    score.lastPointValue = score.defaultPointsScored + (int)timeBonus + (int)streakBonus + (int)comboBonus + (int)score.addBonus;
                    score.score = score.score + score.lastPointValue;

                    if (HasComponent<DamageComponent>(score.scoredAgainstEntity))
                    {
                        //Debug.Log("against " + score.scoredAgainstEntity);
                        var damage = damageGroup[score.scoredAgainstEntity];
                        damage.ScorePointsReceived = score.lastPointValue;
                        damageGroup[score.scoredAgainstEntity] = damage;
                    }

                    score.pointsScored = false;
                    //score.scoringAmmoEntity = Entity.Null;
                    score.timeSinceLastScore = 0;
                }
                else
                {

                    //if (inputController.leftTriggerPressed)
                    //{
                    //  //if (score.lastShotConnected == false) score.streak = 0;
                    //  score.lastShotConnected = false;
                    //}


                    score.timeSinceLastScore += Time.DeltaTime;
                }

                scoreChecked = score.scoreChecked;
                currentScore = score.score;
                scoreComponentAuthoring.ShowLabelScore(score.score);
                scoreComponentAuthoring.ShowLabelStreak(score.streak);
                scoreComponentAuthoring.ShowLabelCombo(score.combo);
                scoreComponentAuthoring.ShowLabelLevel(LevelManager.instance.currentLevelCompleted + 1);

            }
        ).Run();






        bool updateScoreForMenu = SaveManager.instance.updateScore;
        //run rank score loop only when endgame
        if (LevelManager.instance.endGame == true && scoreChecked == 0 || updateScoreForMenu == true)//update score if clicked from menu
        {
            SaveManager.instance.updateScore = false;
            //int slot = SaveManager.instance.saveWorld.lastLoadedSlot - 1;
            int slot = 0;
            List<float> scores = SaveManager.instance.saveData.saveGames[slot].scoreList;
            //Debug.Log("cs " + currentScore);
            if (updateScoreForMenu == false)
            {
                scores.Add(currentScore);
                scoreChecked = 1;
            }
            scores.Sort();
            SaveManager.instance.saveData.saveGames[slot].scoreList = scores;
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
                    scoreComponent.scoreChecked = scoreChecked;
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


        ecb.Playback(EntityManager);
        ecb.Dispose();



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

