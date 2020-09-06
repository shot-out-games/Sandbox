﻿using UnityEngine;
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
public struct WinnerMenuComponent : IComponentData
{
    public bool hide;
    public int levelTargetReachedCounter;
    public int endGameTargetReachedCounter;
    public int npcDeadCounter;

}


public class WinnerMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
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

    [SerializeField]
    private TextMeshProUGUI message;


    void Start()
    {
        //goalTarget = manager.GetComponentData<WinnerComponent>(entity).goalCounterTarget;
        goalTarget = 36;//using this for this game its an optional target only so not suing above commented

        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button

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

    void Update()
    {
        //ShowMenu();
    }


    public void ShowMenu()
    {
        //Debug.Log("end " + manager.GetComponentData<WinnerMenuComponent>(entity).endGameTargetReachedCounter);
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        if (defaultButton)
        {
            defaultButton.Select();
        }

        int npcDead = LevelManager.instance.NpcDead;
        int npcSaved = LevelManager.instance.NpcSaved;
        int totalPossible = LevelManager.instance.potentialGameTargets;

        float f = (float)npcSaved  /  (float)totalPossible * 100f;
        int score = Mathf.FloorToInt(f);

        Debug.Log("f " + f);
        Debug.Log("sc " + score);


        if (score < 3)
        {
            //message.SetText("Curse you!  My plans for domination are ruined");
            message.SetText("The curse is broken");
        }
        else if (score > 99)
        {
            message.SetText("You did it! Or did you? Now GTFO");
        }
        else if (score > 90)
        {
            message.SetText("Yes, you did well. No, that is not good enough");
        }
        else if (score > 75)
        {
            message.SetText("This is not good enough");
        }
        else
        {
            message.SetText("Your success can only be classified as pathetic");
        }

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

        dstManager.AddComponentData(entity, new WinnerMenuComponent()
        {
            hide = true,
            levelTargetReachedCounter = 0,
            endGameTargetReachedCounter = 0
        });
    }
}
