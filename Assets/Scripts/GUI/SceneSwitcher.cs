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
    public float SceneSwitchInterval = 1;
    public int CurrentSceneIndex = 0;
    private EntityManager manager;
    private Entity e;

    //void Start()
    //{
    //    //Debug.Log("scene start ");

    //}

    void Start()
    {
        //Debug.Log("scene awake ");
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //entityManager.AddComponentData(e, new SceneSwitcherComponent());


        // gets the curent screen
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        //Debug.Log("index " + CurrentSceneIndex);

        if (CurrentSceneIndex > 1 && LevelManager.instance.audioSourceMenu)
        {
            LevelManager.instance.audioSourceMenu.Stop();
            LevelManager.instance.PlayLevelMusic();//scene 0 is loader and scene 1  is menu - has own play
        }
        else if (CurrentSceneIndex == 1 && LevelManager.instance.audioSourceGame)
        {
            LevelManager.instance.audioSourceGame.Stop();
            LevelManager.instance.PlayMenuMusic();//scene 0 is loader and scene 1  is menu - has own play
        }
    }

    void Update()
    {


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
        Debug.Log("level loaded " + SceneManager.GetActiveScene().buildIndex);
        LevelManager.instance.InitGameData();

        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var sceneSwitcher = entityManager.GetComponentData<SceneSwitcherComponent>(e);
        //sceneSwitcher.delete = false;
        //manager.SetComponentData<SceneSwitcherComponent>(e, sceneSwitcher);

    }



    public void OnButtonResetGameClicked()
    {
        //bool resetLevel = SaveManager.instance.saveWorld.isSlotSaved[1] == false;
        LevelManager.instance.ClearGameData();
        LevelManager.instance.resetLevel = true;
        LevelManager.instance.currentLevelCompleted = 0;

        SaveLevelManager.instance.saveScene = false;
        SaveManager.instance.saveMainGame = false;
        LevelManager.instance.loadGame = false;
        //LevelManager.instance.skipLoad = true;
        SaveManager.instance.SaveCurrentLevelCompleted(0);
        StartCoroutine(LoadYourAsyncScene(2));

    }



    public void OnButtonSaveAndExitClicked()//called from game pause menu and end game menu - if new project will need to be added to those possibly
    {
        //bool resetLevel = SaveManager.instance.saveWorld.isSlotSaved[1] == false;
        LevelManager.instance.ClearGameData();
        SaveManager.instance.saveMainGame = true;
        SaveManager.instance.saveWorld.isSlotSaved[0] = true;
        SaveManager.instance.SaveWorldSettings();
        //Debug.Log("save and exit " + LevelManager.instance.currentLevelCompleted);
        SaveManager.instance.SaveCurrentLevelCompleted(LevelManager.instance.currentLevelCompleted);
        SaveLevelManager.instance.saveScene = true;
        StartCoroutine(LoadYourAsyncScene(1));


    }


    public void OnButtonExitClicked()//called from game pause menu and end game menu - if new project will need to be added to those possibly
    {
        LevelManager.instance.ClearGameData();
        LevelManager.instance.resetLevel = true;
        StartCoroutine(LoadYourAsyncScene(1));
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

    public void EnableLoadGame()//called only by load slot from load menu
    {
        LevelManager.instance.loadMenuContinueClicked = true;
    }

    public void SetupAndLoadNextScene()
    {
        SaveLevelManager.instance.saveScene = true;
        SaveManager.instance.SaveCurrentLevelCompleted(LevelManager.instance.currentLevelCompleted);
        Debug.Log("setup and load next");
        LoadNextScene();
    }

    public void LoadQuickPlayScene()
    {
        Debug.Log("load quick scene");
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        LevelManager.instance.currentLevelCompleted = 0;
        SaveManager.instance.SaveCurrentLevelCompleted(0);
        var nextSceneIndex = 2;
        StartCoroutine(LoadYourAsyncScene(nextSceneIndex));
        Debug.Log("load next scene complete " + nextSceneIndex);

    }


    public void LoadClickScene()
    {
        Debug.Log("load click scene");
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        int level = SaveManager.instance.saveData.saveGames[0].currentLevel;
        if (LevelManager.instance.newGame == true)
        {
            level = 0;
            LevelManager.instance.newGame = false;
            LevelManager.instance.currentLevelCompleted = level;
            SaveManager.instance.SaveCurrentLevelCompleted(level);
        }
        var nextSceneIndex = level + 2;
        if (nextSceneIndex < 2) nextSceneIndex = 2;//0 is loader 1 is menu
        if (nextSceneIndex >= sceneCount)
        {
            Quit();
            return;
        }
        StartCoroutine(LoadYourAsyncScene(nextSceneIndex));
        Debug.Log("load next scene complete " + nextSceneIndex);
    }

    public void LoadNextScene()
    {
        Debug.Log("load next scene");
        var sceneCount = SceneManager.sceneCountInBuildSettings;
        int level = LevelManager.instance.currentLevelCompleted;
        SaveManager.instance.SaveCurrentLevelCompleted(level);//need to save  because enableload called after then loadsystem will load to entities created when loading next level
        var nextSceneIndex = level + 2;
        if (nextSceneIndex < 2) nextSceneIndex = 2;//0 is loader 1 is menu
        if (nextSceneIndex > sceneCount)
        {
            Quit();
            return;
        }
        StartCoroutine(LoadYourAsyncScene(nextSceneIndex));
        Debug.Log("load next scene complete " + nextSceneIndex);
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

    private IEnumerator Pause(int buildIndex, float secs)
    {
        yield return new WaitForSeconds(secs);
        Debug.Log("post " + buildIndex);
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;

        if (LevelManager.instance.loadMenuContinueClicked == true)
        {
            LevelManager.instance.loadGame = true;
            LevelManager.instance.loadMenuContinueClicked = false;
        }



        manager.AddComponentData(e, new SceneSwitcherComponent());

    }
}



public class ResetLevelSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (SaveManager.instance == null || LevelManager.instance == null) return;


        if (LevelManager.instance.resetLevel == false) return;
        LevelManager.instance.resetLevel = false;

        //Debug.Log("reset level");

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

        if (SaveManager.instance == null || LevelManager.instance == null) return;


        var ecb = new EntityCommandBuffer(Allocator.Temp);


        //var sceneSwitcher = GetSingleton<SceneSwitcherComponent>();
        //bool setupNextScene = sceneSwitcher.saveScene;
        //if (setupNextScene == false) return;
        bool saveScene = SaveLevelManager.instance.saveScene;
        if (saveScene == false) return;
        //if (HasSingleton<SceneSwitcherComponent>() == false) return;


        SaveLevelManager.instance.saveScene = false;
        SaveLevelManager.instance.loadNextScene = true;



        var playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        NativeArray<Entity> playerEntities = playerQuery.ToEntityArray(Allocator.Temp);
        SaveLevelManager.instance.saveLevelPlayers.Clear();

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

            //SaveLevelManager.instance.saveLevelPlayers.Clear();
            SaveLevelManager.instance.saveLevelPlayers.Add(levelPlayers);
            Debug.Log("setup level  ");

        }


        Debug.Log("deleting");

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

