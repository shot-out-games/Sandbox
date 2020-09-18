using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct LevelComponent : IComponentData
{
    public int currentLevel;
    public int maxLevels;
    public int potentialGameTargets;//in some games max of something ie potential saved robots
}



public class LevelManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool mobile;
    [HideInInspector]
    public bool endGame = false;
    [HideInInspector]
    public int potentialGameTargets;//in some games max of something ie potential saved robots
    //[HideInInspector]
    public List<int> potentialCumulativeGameTargets = new List<int>();
    [HideInInspector]
    public int totalLevels;


    public int playersDead;
    public int enemiesDead;
    public int NpcDead;
    public int playersSaved;
    public int enemiesSaved;
    public int NpcSaved;



    public int maxLevels = 4;
    public string worldName = "World";
    public AudioSource audioSourceMenu;
    public AudioSource audioSourceGame;
    private AudioClip levelLoop;//N/A
    [SerializeField]
    private AudioClip menuMusic;

    public static LevelManager instance = null;
    public int currentLevel;
    public List<LevelSettings> levelSettings = new List<LevelSettings>();
    public List<LevelMedia> levelMediaList = new List<LevelMedia>();
    void Awake()
    {
        currentLevel = 0;
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
        totalLevels = levelSettings.Count;
        DontDestroyOnLoad(gameObject);
    }

    public void ClearGameData()
    {
        currentLevel = 0;
        playersDead = 0;
        enemiesDead = 0;
        NpcDead = 0;
        playersSaved = 0;
        enemiesSaved = 0;
        NpcSaved = 0;

        for (int i = 0; i < totalLevels; i++)
        {
            levelSettings[i].playersDead = 0;
            levelSettings[i].enemiesDead = 0;
            levelSettings[i].NpcDead = 0;
            levelSettings[i].playersSaved = 0;
            levelSettings[i].enemiesSaved = 0;
            levelSettings[i].NpcSaved = 0;
            levelSettings[i].points = 0;
            levelSettings[i].completed = false;
        }



    }

    void Start()
    {
        PotentialCumulativeGameTargets();
        //audioSourceGame = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().buildIndex == 1) //Menu is 1
        {
            PlayMenuMusic();
        }
        else
        {
            PlayLevelMusic();
        }

    }

    private void PotentialCumulativeGameTargets()
    {
        potentialCumulativeGameTargets.Clear();
        int counter = 0;
        for (int i = 0; i < totalLevels; i++)
        {
            counter = counter + levelSettings[i].potentialLevelTargets;
            potentialCumulativeGameTargets.Add(counter);
        }

        potentialGameTargets = counter;
    }


    public void PlayMenuMusic()
    {
        if (audioSourceMenu.isPlaying) audioSourceGame.Stop();
        audioSourceMenu.clip = menuMusic;
        audioSourceMenu.Play();
    }

    public void PlayLevelMusic()//called when switching levels either by scene or when switching levels even when same scene
    {
        if (audioSourceGame.isPlaying) audioSourceGame.Stop();


        AudioClip levelMusic = LevelManager.instance.levelMediaList[currentLevel].levelMusic;

        //Debug.Log("Play " + currentLevel);

        audioSourceGame.clip = levelMusic;
        audioSourceGame.Play();
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        //not used yet just use static level manager data instead
        dstManager.AddComponentData(entity, new LevelComponent()
        {
            currentLevel = 0,
            maxLevels = maxLevels,
            potentialGameTargets = potentialGameTargets
        });

    }
}


