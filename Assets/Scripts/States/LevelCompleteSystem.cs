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
    public bool targetReached;
    public bool checkWinCondition;
    public int dieLevel;

}

//[UpdateAfter(typeof(WinnerSystems))]

public class LevelCompleteSystem : SystemBase
{

    protected override void OnUpdate()
    {


        int currentLevelCompleted = LevelManager.instance.currentLevelCompleted;
        int totalGameLevels = LevelManager.instance.totalLevels;
        if (currentLevelCompleted >= totalGameLevels) return;//all levels complete before even checking if level complete below than return since game over basically

        int levelCompleteCounter = 0;//# reached end that are required to complete the level
        bool levelComplete = false;
        Entity playerEntity = Entity.Null;
        string message = "Level Complete";


        if (LevelManager.instance.levelSettings[currentLevelCompleted].levelCompleteScenario ==
            LevelCompleteScenario.TriggerReached)
        {
            int levelCompleteRequired = 1;


            Entities.WithoutBurst().ForEach
            (
                (ref LevelCompleteComponent LevelCompleteComponent, in PlayerComponent player, in Entity entity) =>
                {
                    if (LevelCompleteComponent.targetReached == true)
                    {
                        LevelCompleteComponent.targetReached = false;
                        levelCompleteCounter += 1;
                        playerEntity = entity;
                        Debug.Log("LEVEL COMPLETE " + currentLevelCompleted + 1);
                    }
                }
            ).Run();


            if (levelCompleteCounter >= levelCompleteRequired)
            {
                levelComplete = true;
                message = "Target Reached";
            }

        }

        //to do : add new scenario where destroy set amount determined by level settings

        if (LevelManager.instance.levelSettings[currentLevelCompleted].levelCompleteScenario ==
            LevelCompleteScenario.DestroyAll)
        {

            levelComplete = true;
            message = "Eliminated all ...";

            Entities.WithAll<EnemyComponent>().WithoutBurst().ForEach
            (
                (in Entity e, in LevelCompleteComponent levelCompleteComponent) =>
                {
                    if (levelComplete == false) return;
                    if (HasComponent<DeadComponent>(e)
                    || levelCompleteComponent.dieLevel != currentLevelCompleted)
                    {
                        levelComplete = false;
                    }

                    //if (dead.isDead == false || dead.dieLevel != currentLevelCompleted)
                    //{
                    //    levelComplete = false;
                    //}
                }
            ).Run();

        }


        if (levelComplete == true)
        {
            LevelManager.instance.currentLevelCompleted += 1;
            Debug.Log("LEVEL UP " + LevelManager.instance.currentLevelCompleted);
            Debug.Log("LEVEL TOTAL " + LevelManager.instance.totalLevels);
            if (LevelManager.instance.currentLevelCompleted < LevelManager.instance.totalLevels)
            {

                LevelManager.instance.PlayLevelMusic();
                Entities.WithoutBurst().WithStructuralChanges().ForEach
                (
                    (
                        LevelCompleteMenuGroup LevelCompleteMenuGroup,
                        ref LevelCompleteMenuComponent LevelCompleteMenuComponent) =>
                    {
                        LevelCompleteMenuComponent.levelLoaded = false; //need?
                        SaveLevelManager.instance.levelMenuShown = true;
                        LevelCompleteMenuGroup.ShowMenu(message);
                    }
                ).Run();

                Debug.Log("level complete menu " + LevelManager.instance.currentLevelCompleted);
            }


        }



    }

}



