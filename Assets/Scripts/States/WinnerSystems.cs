using System.Collections.Generic;
using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;

public class BasicWinnerSystem : SystemBase
{

    protected override void OnUpdate()
    {
        if (LevelManager.instance.currentLevelCompleted  < LevelManager.instance.totalLevels ) return;//not yet marked as completed

        bool winner = true;

        Debug.Log("test0");

        Entities.WithAll<EnemyComponent>().WithoutBurst().ForEach
        (
            (in Entity e) =>
            {
                if (winner == false) return;
                if (HasComponent<DeadComponent>(e)) winner = false;//dead component removed when dead and animation complete

                //if (dead.isDead == false)
                //{
                    //winner = false;
                //}
            }
        ).Run();

        if (winner == false) return;
        Debug.Log("winner");
        LevelManager.instance.endGame = true;
        LevelManager.instance.gameResult = GameResult.Winner;

    }
}




[UpdateAfter(typeof(ScoreSystem))]

public class ShowMenuSystem : SystemBase
{

    int score = 0;
    int rank = 0;
    bool showScoresOnMenu = false;

    protected override void OnCreate()
    {
    }


    protected override void OnUpdate()
    {



        if (LevelManager.instance.gameResult == GameResult.Winner ||
            LevelManager.instance.gameResult == GameResult.Loser)
        {

            //grab "last" player score if any
            Entities.WithoutBurst().WithAll<PlayerComponent>().ForEach
            ((in ScoreComponent scoreComponent) =>
            {
                score = scoreComponent.score;
                rank = scoreComponent.rank;
                showScoresOnMenu = true;
            }
            ).Run();


            LevelManager.instance.StopAudioSources();//level manager not an entity so cant use for each to stop audio sources so use this

        }

        if (LevelManager.instance.gameResult == GameResult.Winner)
        {



            Entities.WithoutBurst().ForEach
            (
                (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup) =>
                {
                    if (winnerMenuComponent.hide == true)
                    {
                        winnerMenuGroup.showScoreboard = showScoresOnMenu;
                        winnerMenuGroup.score = score;
                        winnerMenuGroup.rank = rank;
                        winnerMenuGroup.showMenu = true;
                        winnerMenuComponent.hide = false;
                    }
                }
            ).Run();


        }
        else if (LevelManager.instance.gameResult == GameResult.Loser)
        {


             
            Entities.WithoutBurst().ForEach
            (
                (ref DeadMenuComponent deadMenuComponent, in DeadMenuGroup deadMenuGroup) =>
                {
                    if (deadMenuComponent.hide == true)
                    {
                        Debug.Log("show loser");
                        deadMenuGroup.showScoreboard = showScoresOnMenu;
                        deadMenuGroup.score = score;
                        deadMenuGroup.rank = rank;
                        deadMenuGroup.showMenu = true;
                        deadMenuComponent.hide = false;
                    }
                }
            ).Run();



        }
    }
}




