using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;



[System.Serializable]
public struct WinnerComponent : IComponentData
{
    public bool active;
    public int goalCounter;
    public int goalCounterTarget;
    public bool targetReached;
    public bool endGameReached;
    public bool checkWinCondition;
    public int keys;
}


public class WinnerSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {



        bool endGameCompleteCounter = false;


        Entities.WithoutBurst().ForEach
        (
            (WinnerComponent winnerComponent, Entity e) =>
            {
                if (winnerComponent.endGameReached && winnerComponent.active)
                {
                    //kenney
                    if (winnerComponent.keys == 5)
                    {
                        endGameCompleteCounter = true;
                    }
                }


            }
        ).Run();

        int currentLevel = LevelManager.instance.currentLevel;



        if (endGameCompleteCounter)
        {

            Entities.WithoutBurst().ForEach
            (
                (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup) =>
                {
                    LevelManager.instance.levelSettings[currentLevel].completed = true;
                    if (winnerMenuComponent.hide == true)
                    {
                        LevelManager.instance.endGame = true;
                        winnerMenuGroup.ShowMenu();
                        winnerMenuComponent.hide = false;
                    }
                }
            ).Run();

        }


        return default;
    }

}



