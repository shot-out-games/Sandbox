﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct LevelComponent : IComponentData
{
    //public int currentLevelCompleted;
    //public int maxLevels;
    //public int potentialGameTargets;//in some games max of something ie potential saved robots
}

public enum GameResult
{
    None = 0,
    Winner = 1,
    Loser = 2
}


public class LevelManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool endGame = false;
    public GameResult gameResult = GameResult.None;
    [HideInInspector]
    public int potentialGameTargets;//in some games max of something ie potential saved robots
    public List<int> potentialCumulativeGameTargets = new List<int>();
    public int totalLevels;
    public bool resetLevel;
    public bool loadGame;
    public bool loadMenuContinueClicked;

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
    public int currentLevelCompleted;

    public bool skipLoad = false;
    //public bool justCompleted { get; set; }
    public List<LevelSettings> levelSettings = new List<LevelSettings>();
    public List<LevelMedia> levelMediaList = new List<LevelMedia>();
    public bool newGame = false;

    void Awake()
    {

        currentLevelCompleted = 0;
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
        totalLevels = levelSettings.Count;
        if (totalLevels > maxLevels) totalLevels = maxLevels;
        DontDestroyOnLoad(gameObject);
    }

    public void InitGameData()
    {
        //loadGame = true;
        //if (skipLoad == true)//skipLoad when just resetting level - level is reset then loaded and Initgamedata() run  so we check here
        //{
           // loadGame = false;
            //skipLoad = false;
        //}
        endGame = false;
        gameResult = GameResult.None;

    }


    public void ClearGameData()
    {

        //currentLevelCompleted = 0;

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

    public void StopAudioSources()
    {
        audioSourceGame.Stop();
        audioSourceMenu.Stop();
    }

    public void PlayMenuMusic()
    {
        if (!audioSourceMenu) return;
        if (audioSourceMenu.isPlaying) audioSourceGame.Stop();
        audioSourceMenu.clip = menuMusic;
        audioSourceMenu.Play();
    }

    public void PlayLevelMusic()//called when switching levels either by scene or when switching levels even when same scene
    {
        if (!audioSourceGame) return;

        if (audioSourceGame.isPlaying) audioSourceGame.Stop();


        AudioClip levelMusic = LevelManager.instance.levelMediaList[currentLevelCompleted].levelMusic;
        float levelVolume = LevelManager.instance.levelMediaList[currentLevelCompleted].levelVolume;


        //Debug.Log("Play " + currentLevelCompleted);

        audioSourceGame.clip = levelMusic;
        audioSourceGame.volume = levelVolume;
        audioSourceGame.Play();
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {

        //not used yet just use static level manager data instead
        dstManager.AddComponentData(entity, new LevelComponent()
        {
            //currentLevelCompleted = 1,
            //maxLevels = maxLevels,
            //potentialGameTargets = potentialGameTargets
        });

    }
}


