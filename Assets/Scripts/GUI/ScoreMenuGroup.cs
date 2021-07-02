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
using Rewired;
using System;

public struct ScoresMenuComponent : IComponentData
{
    public bool hide;
    public int index;
    //public int rank;
    public int hi1;
    public int hi2;
    public int hi3;
    public int hi4;
    public int hi5;
}


public class ScoreMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
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


    [SerializeField]
    private TextMeshProUGUI score1;
    [SerializeField]
    private TextMeshProUGUI score2;
    [SerializeField]
    private TextMeshProUGUI score3;
    [SerializeField]
    private TextMeshProUGUI score4;
    [SerializeField]
    private TextMeshProUGUI score5;

    public Player player;
    public int playerId = 0; // The Rewired player id of this character
    public static event Action ScoreMenuExitBackClickedEvent;




    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button

        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);


    }

    void Update()
    {
        if (!ReInput.isReady) return;
        if (manager == default) return;
        ShowScore1();
        ShowScore2();
        ShowScore3();
        ShowScore4();
        ShowScore5();

        if (player.GetButtonDown("select") && canvasGroup.interactable)
        {
            OnExitButtonClicked();
            HideMenu();
            //ScoreMenuExitBackClickedEvent?.Invoke();
        }



    }

    public void OnExitButtonClicked()//saved in memory
    {

        //Debug.Log("exit hi");
        ScoreMenuExitBackClickedEvent?.Invoke();
    }


    private void OnEnable()
    {
        PauseMenuGroup.ScoresClickedEvent += UpdateScoreShowMenu;
        //GameInterface.HideMenuEvent += HideMenu;

    }

    private void UpdateScoreShowMenu()
    {
        SaveManager.instance.updateScore = true;
        //Debug.Log("update score");
        ShowMenu();
    }

    private void OnDisable()
    {
        PauseMenuGroup.ScoresClickedEvent -= UpdateScoreShowMenu;

        //GameInterface.HideMenuEvent -= HideMenu;
    }


    public void ResetScores()
    {
        bool hasComponent = manager.HasComponent(entity, typeof(ScoresMenuComponent));
        if (hasComponent == false) return;
        ScoresMenuComponent scores = manager.GetComponentData<ScoresMenuComponent>(entity);
        scores.hi1 = 0;
        scores.hi2 = 0;
        scores.hi3 = 0;
        scores.hi4 = 0;
        scores.hi5 = 0;

        manager.SetComponentData(entity, scores);

        SaveManager.instance.DeleteHighScoreData();


    }


    void ShowScore1()
    {
        bool hasComponent = manager.HasComponent(entity, typeof(ScoresMenuComponent));
        if (hasComponent == false) return;



        int hi1 =
            manager.GetComponentData<ScoresMenuComponent>(entity).hi1;
        score1.text = "First:  " + hi1.ToString();
    }

    void ShowScore4()
    {
        bool hasComponent = manager.HasComponent(entity, typeof(ScoresMenuComponent));
        if (hasComponent == false) return;



        int hi4 =
            manager.GetComponentData<ScoresMenuComponent>(entity).hi4;
        score4.text = "Fourth:  " + hi4.ToString();
    }

    void ShowScore5()
    {
        bool hasComponent = manager.HasComponent(entity, typeof(ScoresMenuComponent));
        if (hasComponent == false) return;



        int hi5 =
            manager.GetComponentData<ScoresMenuComponent>(entity).hi5;
        score5.text = "Fifth:  " + hi5.ToString();
    }

    void ShowScore2()
    {
        bool hasComponent = manager.HasComponent(entity, typeof(ScoresMenuComponent));
        if (hasComponent == false) return;



        int hi2 =
            manager.GetComponentData<ScoresMenuComponent>(entity).hi2;
        score2.text = "Second:  " + hi2.ToString();
    }

    void ShowScore3()
    {
        bool hasComponent = manager.HasComponent(entity, typeof(ScoresMenuComponent));
        if (hasComponent == false) return;



        int hi3 =
            manager.GetComponentData<ScoresMenuComponent>(entity).hi3;
        score3.text = "Third:  " + hi3.ToString();
    }




    public void ShowMenu()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        if (defaultButton)
        {
            defaultButton.Select();
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

        dstManager.AddComponentData(entity, new ScoresMenuComponent
        {
            hide = true,
            index = 0
        });
    }
}

