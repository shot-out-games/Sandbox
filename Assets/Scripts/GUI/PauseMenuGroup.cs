using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class PauseMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{

    public delegate void ActionResume();
    public static event ActionResume ResumeClickedEvent;//gameinterface subscribes to this

    public static event Action OptionsClickedEvent;//this is same as two lines above - action keyword shorthand
    public static event Action SaveExitClickedEvent;
    public static event Action ExitClickedEvent;
    public static event Action ScoresClickedEvent;

    private CanvasGroup canvasGroup = null;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private Button defaultButton;
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button optionsButton;
    [SerializeField]
    private Button saveExitButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button scoresButton;

    private EntityManager manager;
    private Entity e;

    private void OnResumeClickedEvent()
    {
        //subscriber game interface
        ResumeClickedEvent?.Invoke();
    }

    //public static event Action SaveExitClickedEvent;

    private void OnOptionsClickedEvent()
    {
        //subscriber options menu group -> showmenu 
        //subscriber pause menu group -> showmenu(false)
        OptionsClickedEvent?.Invoke();
    }

    private void OnSaveExitClickedEvent()
    {
        //subscriber scene switcher  -> save and exit
        //subscriber pause menu group -> showmenu(false)
        manager.SetComponentData(e, new SaveComponent { value = true });
        StartCoroutine(Wait(.19f));

    }

    private void OnExitClickedEvent()
    {
        ExitClickedEvent?.Invoke();
        //subscriber scene switcher  -> save and exit
        //subscriber pause menu group -> showmenu(false)

    }

    private void OnScoresClickedEvent()
    {
        ScoresClickedEvent?.Invoke();
        //subscriber score menu group -> showmenu(false)

    }


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        defaultButton.Select();
        resumeButton.onClick.AddListener(OnResumeClickedEvent);
        optionsButton.onClick.AddListener(() => ShowMenu(false));
        optionsButton.onClick.AddListener(OnOptionsClickedEvent);
        saveExitButton.onClick.AddListener(OnSaveExitClickedEvent);
        scoresButton.onClick.AddListener(OnScoresClickedEvent);
        exitButton.onClick.AddListener(OnExitClickedEvent);

    }

    private void OnEnable()
    {
        GameInterface.SelectClickedEvent += ShowMenu;
        SkillTreeMenuGroup.PauseGame += SkillTreeMenuPanel;
        ScoreMenuGroup.ScoreMenuExitBackClickedEvent += ResetSelectedButton;
    }

    private void ResetSelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        //defaultButton.Select();//not working
        //Debug.Log("Select");
    }

    private void SkillTreeMenuPanel(bool paused)
    {
        //paused true when skilltree panel visible
        //Debug.Log("stmp " + paused);
        if (paused) HideMenu();
    }

    public void HideMenu()
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnDisable()
    {
        GameInterface.SelectClickedEvent -= ShowMenu;
        SkillTreeMenuGroup.PauseGame -= SkillTreeMenuPanel;
        ScoreMenuGroup.ScoreMenuExitBackClickedEvent -= ResetSelectedButton;
    }


    public void ShowMenu(bool show)//should have separated from HideMenu
    {
        //menuCanvas.GetComponent<Canvas>().gameObject.SetActive(show);
        Canvas.ForceUpdateCanvases();
        if (show == false)
        {
            ResumeClickedEvent?.Invoke();
        }
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = show ? 1 : 0;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        if (defaultButton && show == true)
        {
            defaultButton.Select();
        }


    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        e = entity;
        manager = dstManager;
        manager.AddComponentData(e, new SaveComponent { value = false });
    }



    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        SaveExitClickedEvent?.Invoke();
    }




}







