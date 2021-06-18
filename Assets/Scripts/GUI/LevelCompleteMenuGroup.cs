﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.AI;

[System.Serializable]
public struct LevelCompleteMenuComponent : IComponentData
{
    public bool hide;
    public bool levelLoaded;
    //public bool levelLoaded;
    public int levelTargetReachedCounter;
    public int endGameTargetReachedCounter;

}


public class LevelCompleteMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    public Entity entity;

    AudioSource audioSource;

    private List<Button> buttons;
    public AudioClip clickSound;
    public EventSystem eventSystem;
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Button defaultButton;

    private int goalTarget;
    //[SerializeField]
    //private TextMeshProUGUI label;
    private float showTimeLength = 1.0f;
    private float showTimer = 0f;
    private bool startShowTimer;
    public bool loadNextScene;

    [SerializeField]
    private TextMeshProUGUI message;

    public static event Action LevelCompleteEvent;



    void Start()
    {
        //goalTarget = manager.GetComponentData<LevelCompleteComponent>(entity).goalCounterTarget;
        goalTarget = 36;//using this for this game its an optional target only so not suing above commented

        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button

    }

    void Update()
    {
        if (startShowTimer)
        {
            showTimer += Time.deltaTime;
            if (showTimer > showTimeLength)
            {
                showTimer = 0;
                startShowTimer = false;
                HideMenu();
                if(loadNextScene) LevelCompleteEvent?.Invoke();
            }
        }
    }

  


    public void ShowMenu(string _message)
    {
        startShowTimer = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (defaultButton)
        {
            defaultButton.Select();
        }

        message.SetText(_message);



    }

    public void HideMenu()
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;

    }




    void PlayMenuClickSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity, new LevelCompleteMenuComponent()
        {
            hide = true,
            levelTargetReachedCounter = 0,
            endGameTargetReachedCounter = 0,
            levelLoaded = true
        });
    }
}

