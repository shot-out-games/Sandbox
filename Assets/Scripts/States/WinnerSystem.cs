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


        //nde  and most games needs two - 1 for dead enemy final boss triggers player endgamereached
        int goal = 2;
        int endGameCompleteCounter = 0;
        bool exit = false;

        Entities.WithoutBurst().ForEach
        (
            (ref WinnerComponent winnerComponent, in RatingsComponent RatingsComponent, in Entity e
            ) =>
            {
                if (winnerComponent.active)
                {
                    //kenney
                    if (winnerComponent.keys >= 8 && RatingsComponent.tag == 1)//player
                    {
                        exit = true;
                        Debug.Log("keys ");
                        winnerComponent.endGameReached = true;
                        endGameCompleteCounter += 1;
                    }
                    if (RatingsComponent.tag == 2 && winnerComponent.endGameReached == true)//enemy
                    {
                        endGameCompleteCounter += 1;
                    }
                }


            }
        ).Run();


        //exit = true;
        if (exit == true)
        {
            LevelManager.instance.audioSource.Stop();
        }


        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (in TriggerComponent triggerComponent, in AudioSource audioSource
            ) =>
            {
                if (exit == true && triggerComponent.Type == (int)TriggerType.Home)
                {

                    Debug.Log("boss song");
                    audioSource.Play();
                }
                else if(exit == true)
                {
                    audioSource.Stop();
                }

            }
        ).Run();



        Entities.WithoutBurst().WithStructuralChanges().ForEach
        (
            (in TriggerComponent triggerComponent, in Entity e
            ) =>
            {
                if (exit == true && triggerComponent.Type == (int)TriggerType.Home)
                {

                    EntityManager.DestroyEntity(e);

                }

            }
        ).Run();


        //exit = true;
        if (exit == true)
        {

            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (in StartGameMenuComponent messageMenuComponent, in StartGameMenuGroup messageMenu) =>
                {
                    if (messageMenu.showOnce == false)
                    {
                        messageMenu.showOnce = true;
                        messageMenu.messageString = "Exit is Open ... Boss can be destroyed";
                        messageMenu.ShowMenu();
                    }
                }
            ).Run();

        }










        int currentLevel = LevelManager.instance.currentLevel;



        if (endGameCompleteCounter >= goal)
        {

            Entities.WithoutBurst().ForEach
            (
                (AudioSource audioSource, TriggerComponent trigger) =>
                {
                    audioSource.Stop();
                }
            ).Run();



            Entities.WithoutBurst().ForEach
            (
                (ref WinnerMenuComponent winnerMenuComponent, in WinnerMenuGroup winnerMenuGroup) =>
                {
                    LevelManager.instance.levelSettings[currentLevel].completed = true;
                    Debug.Log("eg ");
                    if (winnerMenuComponent.hide == true)
                    {
                        Debug.Log("win show ");
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



