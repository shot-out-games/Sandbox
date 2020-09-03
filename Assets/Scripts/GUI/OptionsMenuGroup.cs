using Rewired.UI.ControlMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;



//[System.Serializable]
//public class SoundSliderEvent : UnityEvent<float>
//{
//}



public class OptionsMenuGroup : MonoBehaviour
{



    public static event Action OptionsExitBackClickedEvent;


    private InputController inputController;
    private List<Button> buttons;
    private List<Resolution> resolutions = new List<Resolution>();
    private Resolution currentResolution;
    private List<string> qualityList = new List<string> { "Low", "Medium", "High" };

    [SerializeField] private EventSystem eventSystem;
    [SerializeField]
    private CanvasGroup optionsCanvasGroup = null;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button defaultButton;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField]
    Sprite graphicsIcon;



    [Header("Audio")]
    [SerializeField]
    AudioSource audioSource;
    public AudioClip clickSound;
    public AudioMixer audioMixer;
    [SerializeField]
    private Slider musicSlider = null;
    [SerializeField]
    private Slider soundSlider = null;





    [Header("Resolution")]
    [SerializeField]
    bool resInactive;
    [SerializeField]
    TextMeshProUGUI resText;
    [SerializeField]
    Button resButton;
    [SerializeField]
    private TextMeshProUGUI resLabelNormal;
    [SerializeField]
    private TextMeshProUGUI resLabelHighlighted;
    [SerializeField] private Color resLabelDefaultColor;
    [SerializeField]
    int resButtonIndex;
    private bool resButtonPressed;


    [Header("Quality")]
    [SerializeField]
    Button qualityButton;
    [SerializeField]
    private TextMeshProUGUI qualityLabelNormal;
    [SerializeField]
    private TextMeshProUGUI qualityLabelHighlighted;
    [SerializeField] private Color qualityLabelDefaultColor;
    int qualityButtonIndex;
    private bool qualityButtonPressed;
    [SerializeField] private UniversalRenderPipelineAsset lowQuality = null;
    [SerializeField] private UniversalRenderPipelineAsset mediumQuality = null;
    [SerializeField] private UniversalRenderPipelineAsset highQuality = null;


    private int resWidth = 0;
    private int resHeight = 0;
    private bool isFullScreen = false;
    [SerializeField]
    private bool ignoreGraphics;



    private void OnEnable()
    {
        PauseMenuGroup.OptionsClickedEvent += ShowMenu;
        //GameInterface.HideMenuEvent += HideMenu;

    }

    private void OnDisable()
    {
        PauseMenuGroup.OptionsClickedEvent -= ShowMenu;
        //GameInterface.HideMenuEvent -= HideMenu;
    }

    void Start()
    {
        resLabelDefaultColor = resLabelHighlighted.color;
        qualityLabelDefaultColor = qualityLabelHighlighted.color;
        optionsCanvasGroup = GetComponent<CanvasGroup>();
        inputController = GetComponent<InputController>();
        SaveWorld sw = SaveManager.instance.saveWorld;
        soundSlider.value = sw.soundVolume;
        musicSlider.value = sw.musicVolume;
        int currentQualityIndex = sw.graphicsQuality - 1;
        if (currentQualityIndex < 0) currentQualityIndex = 2;//default high
        buttons = GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayButtonClickSound(clickSound)));


        if (fullscreenToggle && ignoreGraphics == false)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        foreach (var res in Screen.resolutions)
        {
            if (res.refreshRate > 58.99f && res.refreshRate < 61.01f)
            {
                resolutions.Add(res);
            }
            if (res.refreshRate > 73.99f && res.refreshRate < 76.01f)
            {
                resolutions.Add(res);
            }
        }


        float width = sw.screenResWidth == 0 ? Screen.currentResolution.width : sw.screenResWidth;
        float height = sw.screenResHeight == 0 ? Screen.currentResolution.height : sw.screenResHeight;
        string option;
        int currentResIndex = 0;
//        Debug.Log("w " + width);
   //     Debug.Log("h " + height);
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == width &&
                resolutions[i].height == height)
            {
                currentResIndex = i;
            }
            option = resolutions[i].width + "  x " + resolutions[i].height;
            currentResolution = resolutions[i];//not used 
        }

        resWidth = resolutions[currentResIndex].width;
        resHeight = resolutions[currentResIndex].height;
        option = resWidth + "  x " + resHeight;
        resLabelNormal.text = option;
        resLabelHighlighted.text = option;

        qualityLabelNormal.text = qualityList[currentQualityIndex];
        qualityLabelHighlighted.text = qualityList[currentQualityIndex];

        fullscreenToggle.isOn = sw.isFullScreen;
        isFullScreen = fullscreenToggle.isOn;

        resButtonIndex = currentResIndex;
        qualityButtonIndex = currentQualityIndex;

        if (resInactive == true)
        {
            resButton.gameObject.SetActive(false);
            resText.gameObject.SetActive(false);
        }


        //init
        if (ignoreGraphics == false)
        {
            Screen.fullScreen = isFullScreen;
            OnQualitySettingsChanged();
            OnScreenResChanged();
        }
        else
        {
            Screen.fullScreen =  false;
        }

    }



    private void PlayButtonClickSound(AudioClip clip)
    {
        if (!audioSource) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void OnResButtonClicked()
    {
        resButtonPressed = !resButtonPressed;
    }

    public void OnQualityButtonClicked()
    {
        qualityButtonPressed = !qualityButtonPressed;
    }

    private void Update()
    {
        if (!inputController || optionsCanvasGroup == null || GetComponent<CanvasGroup>() == null || eventSystem == null) return;
        if (eventSystem.currentSelectedGameObject == null) return;

        if (inputController.buttonSelect_Pressed && optionsCanvasGroup.interactable)
        {
            OnExitButtonClicked();
            HideMenu();
            OptionsExitBackClickedEvent?.Invoke();
        }

        audioMixer.SetFloat("musicVolume", musicSlider.value * .8f - 80);//0 to 100 slider -80 to 0 db
        audioMixer.SetFloat("soundVolume", soundSlider.value * .8f - 80);//0 to 100 slider -80 to 0 db


        var selected = eventSystem.currentSelectedGameObject.name;
        if (selected == resButton.name)
        {
            if (inputController.buttonA_Pressed) resButtonPressed = !resButtonPressed;
            if (resButtonPressed)
            {
                resLabelHighlighted.color = new Color32(21, 132, 222, 255);
                ChangeResolutionLeftStick();
            }
            else
            {
                resLabelHighlighted.color = resLabelDefaultColor;
                eventSystem.sendNavigationEvents = true;
            }
        }

//        qualityLabelHighlighted.color = Color.red;


        if (selected == qualityButton.name)
        {
            if (inputController.buttonA_Pressed) qualityButtonPressed = !qualityButtonPressed;
            if (qualityButtonPressed)
            {
                qualityLabelHighlighted.color = new Color32(21, 132, 222, 255);
                ChangeQualityLeftStick();
            }
            else
            {
                qualityLabelHighlighted.color = qualityLabelDefaultColor;
                eventSystem.sendNavigationEvents = true;
            }
        }

    }

    private void ChangeResolutionLeftStick()
    {

        eventSystem.sendNavigationEvents = false;


        float v = inputController.leftStickY;
        if (inputController.leftStickYreleased == false) return;

        if (v > .19)
        {
            if (resButtonIndex > 0) resButtonIndex -= 1;
            OnScreenResChanged();
        }
        else if (v < -.19)
        {
            if (resButtonIndex < resolutions.Count - 1) resButtonIndex += 1;
            OnScreenResChanged();
        }

        string option = resolutions[resButtonIndex].width + "  x " + resolutions[resButtonIndex].height;
        resLabelNormal.text = option;
        resLabelHighlighted.text = option;
    }

    private void ChangeQualityLeftStick()
    {
        eventSystem.sendNavigationEvents = false;
        float v = inputController.leftStickY;
        if (inputController.leftStickYreleased == false) return;

        if (v > .19)
        {
            if (qualityButtonIndex > 0) qualityButtonIndex -= 1;
            OnQualitySettingsChanged();
        }
        else if (v < -.19)
        {
            if (qualityButtonIndex < qualityList.Count - 1) qualityButtonIndex += 1;
            OnQualitySettingsChanged();
        }

        string option = qualityList[qualityButtonIndex];
        qualityLabelNormal.text = option;
        qualityLabelHighlighted.text = option;



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
        //Debug.Log("hide options ");
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
        SaveManager.instance.saveWorld.graphicsQuality = qualityButtonIndex + 1;
        SaveManager.instance.saveWorld.screenResHeight = resHeight;
        SaveManager.instance.saveWorld.screenResWidth = resWidth;
        SaveManager.instance.saveWorld.isFullScreen = isFullScreen;
        //Debug.Log("fs " + isFullScreen);
        //Debug.Log("h " + resHeight);
        //Debug.Log("w " + resWidth);


    }

    public void OnQualitySettingsChanged()
    {
        if (qualityButtonIndex == 0)
        {
            GraphicsSettings.renderPipelineAsset = lowQuality;
        }
        else if (qualityButtonIndex == 1)
        {
            GraphicsSettings.renderPipelineAsset = mediumQuality;
        }
        else if (qualityButtonIndex == 2)
        {
            GraphicsSettings.renderPipelineAsset = highQuality;
        }

        //SetQuality();
    }

    public void OnFullscreenChanged(bool _isFullscreen)
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void OnScreenResChanged()
    {
        if(resInactive == true) return;
        //int index = ResDropdown.index;
        Resolution resolution = resolutions[resButtonIndex];
        resHeight = resolution.height;
        resWidth = resolution.width;
        Screen.SetResolution(resWidth, resHeight, Screen.fullScreen);
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







