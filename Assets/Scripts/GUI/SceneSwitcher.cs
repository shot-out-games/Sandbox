using System;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;



public struct SceneSwitcherComponent : IComponentData
{
    public bool delete;
    public bool saveScene;
}

public class SceneSwitcher : MonoBehaviour, IConvertGameObjectToEntity
{
    [Serializable]
    public class SceneConfig
    {
        public string SceneName;
        public int CustomDuration;
    }
    public int SceneSwitchInterval = 4;
    public int CurrentSceneIndex = 0;
    private EntityManager manager;
    private Entity e;

    private bool sceneLoaded;

    void Start()
    {
        Debug.Log("scene start ");

    }

    void Awake()
    {
        Debug.Log("scene awake ");
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //entityManager.AddComponentData(e, new SceneSwitcherComponent());


        // gets the curent screen
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("index " + CurrentSceneIndex);

        if (CurrentSceneIndex > 1)
        {
            LevelManager.instance.audioSourceMenu.Stop();
            LevelManager.instance.PlayLevelMusic();//scene 0 is loader and scene 1  is menu - has own play
        }
        else if (CurrentSceneIndex == 1)
        {
            LevelManager.instance.audioSourceGame.Stop();
            LevelManager.instance.PlayMenuMusic();//scene 0 is loader and scene 1  is menu - has own play
        }
    }

    void Update()
    {
        if (sceneLoaded == true)
        {
            manager.SetComponentData(e, new LoadComponent { part1 = true, part2 = true });
            sceneLoaded = false;
        }

    }

    void OnEnable()
    {
        PauseMenuGroup.SaveExitClickedEvent += OnButtonSaveAndExitClicked;
        PauseMenuGroup.ExitClickedEvent += OnButtonExitClicked;
        LevelCompleteMenuGroup.LevelCompleteEvent += SetupAndLoadNextScene;
        SceneManager.sceneLoaded += OnLevelFinishedLoading;

    }


    void OnDisable()
    {
        PauseMenuGroup.SaveExitClickedEvent -= OnButtonSaveAndExitClicked;
        PauseMenuGroup.ExitClickedEvent -= OnButtonExitClicked;
        LevelCompleteMenuGroup.LevelCompleteEvent -= SetupAndLoadNextScene;
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }


    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("level loaded");
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var sceneSwitcher = entityManager.GetComponentData<SceneSwitcherComponent>(e);
        //sceneSwitcher.delete = false;
        //manager.SetComponentData<SceneSwitcherComponent>(e, sceneSwitcher);

    }



    public void OnButtonResetGameClicked()
    {
        LevelManager.instance.ClearGameData();
        LevelManager.instance.resetLevel = true;
        StartCoroutine(LoadYourAsyncScene(CurrentSceneIndex));
    }


    public void Quit()
    {
        SaveManager.instance.SaveWorldSettings();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }


    public void OnButtonSaveAndExitClicked()//called from game pause menu and end game menu - if new project will need to be added to those possibly
    {
        LevelManager.instance.ClearGameData();
        SaveManager.instance.SaveWorldSettings();
        SaveManager.instance.SaveGameData();
        DestroyAllEntitiesInScene();
        StartCoroutine(LoadYourAsyncScene(1));


    }


    public void OnButtonExitClicked()//called from game pause menu and end game menu - if new project will need to be added to those possibly
    {
        LevelManager.instance.ClearGameData();
        DestroyAllEntitiesInScene();
        StartCoroutine(LoadYourAsyncScene(1));
    }


    private void DestroyAllEntitiesInScene()
    {

        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var entities = entityManager.GetAllEntities();
        //entityManager.DestroyEntity(entities);
        //entities.Dispose();

        //var sceneSwitcher = manager.GetComponentData<SceneSwitcherComponent>(e);
        //sceneSwitcher.delete = true;//no setup scene field first - then delete in system
        //sceneSwitcher.saveScene = true;
        //manager.SetComponentData<SceneSwitcherComponent>(e, sceneSwitcher);

        SaveLevelManager.instance.saveScene = true;


    }



    IEnumerator LoadYourAsyncScene(int sceneIndex)
    {


        yield return new WaitForSeconds(SceneSwitchInterval);
        //yield return new WaitForSeconds(20);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads

        while (!asyncOperation.isDone)
        {
            //Output the current progress
            Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Activate the Scene
                asyncOperation.allowSceneActivation = true;

            }

            yield return null;

        }

    }

    private void SetupAndLoadNextScene()
    {
        DestroyAllEntitiesInScene();
        LoadNextScene();
    }


    public void LoadNextScene()
    {
        Debug.Log("load next");
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        var nextIndex = CurrentSceneIndex + 1;
        if (nextIndex >= sceneCount)
        {
            Quit();
            return;
        }



        var nextScene = SceneUtility.GetScenePathByBuildIndex(nextIndex);
        //TimeUntilNextSwitch = GetSceneDuration(nextScene);
        CurrentSceneIndex = nextIndex;
        StartCoroutine(LoadYourAsyncScene(nextIndex));
        //Debug.Log("load next scene complete");
    }


    private IEnumerator Pause(int buildIndex, float secs)
    {
        yield return new WaitForSeconds(secs);
        Debug.Log("post " + buildIndex);
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        Debug.Log("add scene switcher component " + e);
        if (CurrentSceneIndex == 2)
        {
            sceneLoaded = true;
            manager.AddComponentData(e, new LoadComponent { e = entity, part1 = false, part2 = false });
        }

        manager.AddComponentData(e, new SceneSwitcherComponent());

    }
}



public class ResetLevelSystem : SystemBase
{
    protected override void OnUpdate()
    {

        if(LevelManager.instance.resetLevel == false) return;
        LevelManager.instance.resetLevel = false;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity e) =>
        {
            ecb.DestroyEntity(e);

        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

    }
}






public class SetupNextLevelSystem : SystemBase
{





    protected override void OnUpdate()
    {
        if (HasSingleton<SceneSwitcherComponent>() == false) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);


        //var sceneSwitcher = GetSingleton<SceneSwitcherComponent>();
        //bool setupNextScene = sceneSwitcher.saveScene;
        //if (setupNextScene == false) return;
        bool saveScene = SaveLevelManager.instance.saveScene;
        if (saveScene == false) return;

        SaveLevelManager.instance.saveScene = false;
        SaveLevelManager.instance.loadNextScene = true;

        var playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Temp);

        for (int i = 0; i < playerEntities.Length; i++)
        {
            Entity playerEntity = playerEntities[i];
            var savedPlayer = GetComponent<PlayerComponent>(playerEntity);//required
            var savedHealth = new HealthComponent();
            if (HasComponent<HealthComponent>(playerEntity))
                savedHealth = GetComponent<HealthComponent>(playerEntity);
            var savedStats = new StatsComponent();
            if (HasComponent<StatsComponent>(playerEntity))
                savedStats = GetComponent<StatsComponent>(playerEntity);
            var savedScores = new ScoreComponent();
            if (HasComponent<ScoreComponent>(playerEntity))
                savedScores = GetComponent<ScoreComponent>(playerEntity);


            var levelPlayers = new SaveLevelPlayers()
            {
                playerLevelData = new PlayerLevelData()
                {
                    savedLevelHealth = savedHealth,
                    savedLevelPlayer = savedPlayer,
                    savedLevelStats = savedStats,
                    savedLevelScores = savedScores


                }
            };

            SaveLevelManager.instance.saveLevelPlayers.Add(levelPlayers);
            Debug.Log("setup level  ");

        }


        //Debug.Log("deleting");

        Entities.WithoutBurst().ForEach((Entity _e) =>
        {
            ecb.DestroyEntity(_e);
        }
        ).Run();

        //Entity e = GetSingletonEntity<SceneSwitcherComponent>();
        //sceneSwitcher.saveScene = false;
        //sceneSwitcher.delete = true;
        //ecb.SetComponent<SceneSwitcherComponent>(e, sceneSwitcher);
        //ecb.DestroyEntity(e);



        ecb.Playback(EntityManager);
        ecb.Dispose();




    }

}

