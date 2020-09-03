using System;
using Unity.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.Jobs;

public struct GameInterfaceComponent : IComponentData
{
    public int pause;
}

public struct Pause : IComponentData
{
    public int value;
}

public class GameInterface : MonoBehaviour, IConvertGameObjectToEntity
{

    public delegate void ActionSelect(bool paused);
    public static event ActionSelect SelectClickedEvent;
    public static event Action HideMenuEvent;
    public bool paused = false;


    private void OnEnable()
    {
        PauseMenuGroup.ResumeClickedEvent += ResumeClicked;
        PauseMenuGroup.OptionsClickedEvent += OptionsClicked;
        OptionsMenuGroup.OptionsExitBackClickedEvent += OptionsExitClicked;
        SkillTreeMenuGroup.PauseGame += OtherMenu;
    }

    private void OnDisable()
    {
        PauseMenuGroup.ResumeClickedEvent -= ResumeClicked;
        SkillTreeMenuGroup.PauseGame -= OtherMenu;
        PauseMenuGroup.OptionsClickedEvent -= OptionsClicked;
    }

    void Start()
    {

    }

    private void OptionsExitClicked()
    {
        paused = false;
    }

    private void OtherMenu(bool pause)
    {
        paused = pause;
    }



    public void SelectClicked()//only called with button from system no menu item currently
    {
        paused = !paused;
        SelectClickedEvent?.Invoke(paused);//pause menu subscribes to this event to show pause menu
        HideMenuEvent?.Invoke();
    }

    public void ResumeClicked()
    {
        paused = false;
    }

    public void OptionsClicked()
    {
        paused = true;//force pause when pause menu options clicked
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<GameInterfaceComponent>(entity, new GameInterfaceComponent { pause = 0 });
    }
}




public class GameInterfaceSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        bool paused = false;
        bool selectPressed = false;

        //if(EntityManager.HasComponent<DeadMenuComponent>())

        bool required = HasSingleton<DeadMenuComponent>() && HasSingleton<WinnerMenuComponent>();
        if (required == false) return  default;

        bool deadMenuDisplayed = !GetSingleton<DeadMenuComponent>().hide;
        bool winnerMenuDisplayed = !GetSingleton<WinnerMenuComponent>().hide;



        Entities.WithoutBurst().ForEach
            (
                (
                    Entity entity,
                    InputController inputController,
                    GameInterface gameInterface

                ) =>
                {
                    selectPressed = inputController.buttonSelect_Pressed;
                    if (selectPressed && deadMenuDisplayed == false && winnerMenuDisplayed == false)
                    {
                        gameInterface.SelectClicked();
                    }
                    paused = gameInterface.paused;//should probably be component
                }

            ).Run();

        int pause = paused ? 1 : 0;
        Entities.WithoutBurst().ForEach((Animator animator, Entity entity) =>
        {
            animator.speed = paused ? 0 : 1; ;
            EntityManager.SetComponentData<Pause>(entity, new Pause { value = pause });
        }
        ).Run();


        Entities.WithoutBurst().ForEach((EnemyMove enemyMove, Entity entity, NavMeshAgent agent, RatingsComponent enemyRatings, DeadComponent dead) =>
        {
            enemyMove.moveSpeed = paused || dead.isDead ? 0 : enemyRatings.speed;
            //agent.speed = enemyMove.moveSpeed;
            EntityManager.SetComponentData<Pause>(entity, new Pause { value = pause });
        }
        ).Run();

        return default;

    }
}


