using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;



public class OptionsMenuGroup : MonoBehaviour
{

    public Player player;
    public int playerId = 0; // The Rewired player id of this character
    public static event Action OptionsExitBackClickedEvent;
    private List<Button> buttons;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField]
    private CanvasGroup optionsCanvasGroup = null;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button defaultButton;


    [Header("Audio")]
    [SerializeField]
    AudioSource audioSource;
    public AudioClip clickSound;
    public AudioMixer audioMixer;
    [SerializeField]
    private Slider musicSlider = null;
    [SerializeField]
    private Slider soundSlider = null;



    private void OnEnable()
    {
        PauseMenuGroup.OptionsClickedEvent += ShowMenu;
    }

    private void OnDisable()
    {
        PauseMenuGroup.OptionsClickedEvent -= ShowMenu;
    }

    void Start()
    {

        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(playerId);
        optionsCanvasGroup = GetComponent<CanvasGroup>();
        SaveWorld sw = SaveManager.instance.saveWorld;
        soundSlider.value = sw.soundVolume;
        musicSlider.value = sw.musicVolume;
        //int currentQualityIndex = sw.graphicsQuality - 1;
        //if (currentQualityIndex < 0) currentQualityIndex = 2;//default high
        buttons = GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayButtonClickSound(clickSound)));


    }



    private void PlayButtonClickSound(AudioClip clip)
    {
        if (!audioSource) return;
        audioSource.clip = clip;
        audioSource.Play();
    }


    private void Update()
    {


        if (optionsCanvasGroup == null || GetComponent<CanvasGroup>() == null || eventSystem == null) return;
        if (eventSystem.currentSelectedGameObject == null) return;

        if (player.GetButtonDown("select") && optionsCanvasGroup.interactable)
        {
            OnExitButtonClicked();
            HideMenu();
            OptionsExitBackClickedEvent?.Invoke();
        }

        audioMixer.SetFloat("musicVolume", musicSlider.value * .8f - 80);//0 to 100 slider -80 to 0 db
        audioMixer.SetFloat("soundVolume", soundSlider.value * .8f - 80);//0 to 100 slider -80 to 0 db



    }

    public void ShowMenu()
    {
        optionsCanvasGroup.interactable = true;
        optionsCanvasGroup.alpha = 1;
        optionsCanvasGroup.blocksRaycasts = true;
        if (defaultButton)
        {
            defaultButton.Select();
        }

    }

    public void HideMenu()
    {
        if (GetComponent<CanvasGroup>() == null || optionsCanvasGroup == null) return;//gets destroyed sometimes ???
        eventSystem.sendNavigationEvents = true;
        optionsCanvasGroup = GetComponent<CanvasGroup>();
        optionsCanvasGroup.interactable = false;
        optionsCanvasGroup.alpha = 0.0f;
        optionsCanvasGroup.blocksRaycasts = false;

    }


    public void OnExitButtonClicked()//saved in memory
    {

        OptionsExitBackClickedEvent?.Invoke();


        SaveManager.instance.saveWorld.musicVolume = musicSlider.value;
        SaveManager.instance.saveWorld.soundVolume = soundSlider.value;

    }

    public void OnMusicSliderValueChanged(float musicVolume)
    {
        float value = musicVolume * .8f - 80f;
        audioMixer.SetFloat("musicVolume", value);//0 to 100 slider -80 to 0 db

    }

    public void OnSoundSliderValueChanged(float soundVolume)
    {
        float value = soundVolume * .8f - 80f;
        audioMixer.SetFloat("soundVolume", value);
        Debug.Log("so " + value);
    }

}







