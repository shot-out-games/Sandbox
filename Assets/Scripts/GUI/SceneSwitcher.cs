using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        // gets the curent screen
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log("index " + CurrentSceneIndex);

        if (CurrentSceneIndex > 1) LevelManager.instance.PlayLevelMusic();//scene 0 is loader and scene 1  is menu - has own play
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
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        PauseMenuGroup.SaveExitClickedEvent -= OnButtonSaveAndExitClicked;
        PauseMenuGroup.ExitClickedEvent -= OnButtonExitClicked;
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }


    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //StartCoroutine(Pause(scene.buildIndex, 2.0f));
        //manager.SetComponentData(e, new LoadComponent { value = true });
        //sceneLoaded = true;


    }



    public void OnButtonResetGameClicked()
    {
        LevelManager.instance.ClearGameData();
        DestroyAllEntitiesInScene();
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

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entities = entityManager.GetAllEntities();
        entityManager.DestroyEntity(entities);
        entities.Dispose();
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



    public void LoadNextScene()
    {
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
        Debug.Log("load next scene complete");
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
        if (CurrentSceneIndex == 2)
        {
            sceneLoaded = true;
            manager.AddComponentData(e, new LoadComponent {e = entity, part1 = false, part2 = false});
        }


    }
}

