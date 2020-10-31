using Unity.Entities;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;


//mish mash
//not consistent using entity components for some and static level settings (easier) for others

[System.Serializable]
public struct LevelCompleteComponent : IComponentData
{
    public bool active;
    public int goalCounter;
    public int goalCounterTarget;
    public bool targetReached;
    public bool checkWinCondition;

}

//[UpdateAfter(typeof(WinnerSystems))]

public class LevelCompleteSystem : SystemBase
{

    protected override  void OnUpdate()
    {

        int levelCompleteCounter = 0;//# reached end that are required to complete the level
        int levelCompleteRequired = 1;
        int currentLevel = LevelManager.instance.currentLevel;
        int totalGameLevels = LevelManager.instance.totalLevels;
        if (LevelManager.instance.levelSettings.Count < totalGameLevels)
            totalGameLevels = LevelManager.instance.levelSettings.Count;

        Entity playerEntity = Entity.Null;

        Entities.WithoutBurst().ForEach
        (
            (ref LevelCompleteComponent LevelCompleteComponent, in PlayerComponent player, in Entity entity) =>
            {
                if (LevelCompleteComponent.targetReached)
                {
                    levelCompleteCounter += 1;
                    playerEntity = entity;
                    Debug.Log("LEVEL COMPLETE " + currentLevel);
                }

            }
        ).Run();




        if (levelCompleteCounter >= levelCompleteRequired)
        {


            Entities.WithoutBurst().ForEach
            (
                (ref LevelCompleteMenuComponent LevelCompleteMenuComponent, in LevelCompleteMenuGroup LevelCompleteMenuGroup) =>
                {
                    LevelManager.instance.levelSettings[currentLevel].completed = true;
                    if (LevelManager.instance.endGame == false)
                    {
                        var playerLevelComplete = EntityManager.GetComponentData<LevelCompleteComponent>(playerEntity);//only works on one player level complete
                        playerLevelComplete.targetReached = false;
                        //Debug.Log("levl com show menu ");
                        EntityManager.SetComponentData(playerEntity, playerLevelComplete);
                        LevelCompleteMenuGroup.ShowMenu();
                    }
                }
            ).Run();





            if (totalGameLevels > currentLevel + 1 )
            {
                LevelManager.instance.currentLevel += 1;
                Debug.Log("LEVEL UP " + LevelManager.instance.currentLevel);
                LevelManager.instance.PlayLevelMusic();
            }




        }


        //return default;
    }

}



