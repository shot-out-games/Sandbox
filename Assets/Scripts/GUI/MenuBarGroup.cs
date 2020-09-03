using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MenuBarGroup : MonoBehaviour
{

//    public delegate void ActionResume();
    //public static event ActionResume ResumeClickedEvent;//gameinterface subscribes to this

//    public static event Action OptionsClickedEvent;//this is same as two lines above - action keyword shorthand

    //public static event Action SaveExitClickedEvent;

    //[SerializeField] private GameObject menuCanvas;
    private CanvasGroup canvasGroup = null;

    //[SerializeField]
    //private List<Button> buttons;

    [SerializeField]
    private Button defaultButton;
    //[SerializeField]
    //private Button resumeButton;
    //[SerializeField]
    //private Button optionsButton;
    //[SerializeField]
    //private Button saveExitButton;


    //private UnityAction<bool> action;


    //private static void OnResumeClickedEvent()
    //{
    //    //subscriber game interface
    //    ResumeClickedEvent?.Invoke();
    //}

    ////public static event Action SaveExitClickedEvent;

    //private static void OnOptionsClickedEvent()
    //{
    //    //subscriber options menu group -> showmenu 
    //    //subscriber pause menu group -> showmenu(false)
    //    OptionsClickedEvent?.Invoke();
    //}

    //private static void OnSaveExitClickedEvent()
    //{
    //    //subscriber scene switcher  -> save and exit
    //    //subscriber pause menu group -> showmenu(false)
    //    SaveExitClickedEvent?.Invoke();
    //}

    [SerializeField] private EventSystem eventSystem;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        defaultButton.Select();

        //resumeButton.onClick.AddListener(OnResumeClickedEvent);

        //optionsButton.onClick.AddListener(()=>ShowMenu(false));
        //optionsButton.onClick.AddListener(OnOptionsClickedEvent);

        //saveExitButton.onClick.AddListener(OnSaveExitClickedEvent);

    }

    private void OnEnable()
    {
        PauseMenuGroup.OptionsClickedEvent += ShowMenu;
        GameInterface.HideMenuEvent += HideMenu;

    }

    private void OnDisable()
    {
        PauseMenuGroup.OptionsClickedEvent -= ShowMenu;
        GameInterface.HideMenuEvent -= HideMenu;


    }





    public void ShowMenu()
    {
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        if (defaultButton)
        {
            defaultButton.Select();
        }

    }

    public void HideMenu()
    {
        if (GetComponent<CanvasGroup>() == null || canvasGroup == null) return;//gets destroyed sometimes ???
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;

    }

}







