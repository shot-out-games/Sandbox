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

        bool winner = true;


        Entities.WithAll<EnemyComponent>().WithoutBurst().ForEach
        (
            (in DeadComponent dead) =>
            {
                if (winner == false) return;
                if (dead.isDead == false)
                {
                    winner = false;
                }
            }
        ).Run();

        //winner = true;

        if (winner == false) return;

        LevelManager.instance.endGame = true;
        LevelManager.instance.gameResult = GameResult.Winner;

    }
}




[UpdateAfter(typeof(ScoreSystem))]

public class ShowMenuSystem : SystemBase
{


    protected override void OnUpdate()
    {

        if (LevelManager.instance.gameResult == GameResult.Winner)
        {

            //grab "last" player score if any
            int score = 0;
            int rank = 0;
            bool showScoresOnWinScreen = false;
            Entities.WithoutBurst().WithAll<PlayerComponent>().ForEach
            ((ScoreComponent scoreComponent) =>
                {
                    score = scoreComponent.score;
                    rank = scoreComponent.rank;
                    showScoresOnWinScreen = true;
                    Debug.Log("ss0 " + showScoresOnWinScreen);
                }
            ).Run();


            Entities.WithoutBurst().ForEach
            (
                (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup) =>
                {
                    if (winnerMenuComponent.hide == true)
                    {
                        Debug.Log("ss1 " + showScoresOnWinScreen);
                        winnerMenuGroup.score = score;
                        winnerMenuGroup.rank = rank;
                        winnerMenuGroup.ShowMenu(showScoresOnWinScreen);
                        winnerMenuComponent.hide = false;
                    }
                }
            ).Run();







        }

    }
}




