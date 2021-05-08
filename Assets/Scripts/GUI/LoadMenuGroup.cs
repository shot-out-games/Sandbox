using System.Collections;
using Rewired;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{
    private CanvasGroup canvasGroup;
    public UnityEvent OnBackClicked = new UnityEvent();
    private int selectedSlot = 0;
    [SerializeField]
    Button loadButton;

    private Player player;
    [SerializeField]
    private TextMeshProUGUI slotText;
    [SerializeField]
    private TextMeshProUGUI slotTextHighlighted;


    void Start()
    {
        if (!ReInput.isReady) return;
        player = ReInput.players.GetPlayer(0);
        canvasGroup = GetComponent<CanvasGroup>();
        UpdateButtons();

    }

    public void UpdateButtons()
    {
        if (SaveManager.instance.saveData.saveGames.Count == 0) return;
        SaveWorld sw = SaveManager.instance.saveWorld;

        if (sw.isSlotSaved[0] == true)
        {
            slotText.text = "CONTINUE";
            LevelManager.instance.newGame = false;
        }
        else
        {
            slotText.text = "New Game";
            LevelManager.instance.newGame = true;
        }

        slotTextHighlighted.text = sw.isSlotSaved[0] ? "CONTINUE " : "New Game";
    }

    public void OnLoadSlotClicked(int slot)
    {
        selectedSlot = slot;
        OnLoadOptionClicked();
    }

    public void OnLoadOptionClicked()
    {
        loadButton.interactable = false;
        LevelManager.instance.resetLevel = true;
        SaveManager.instance.LoadSaveWorld();
        SaveData sd = SaveManager.instance.LoadSaveData();
        int level = sd.saveGames[0].currentLevel;
        SaveManager.instance.SaveCurrentLevelCompleted(level);
        LevelManager.instance.currentLevelCompleted = level;
        if(SaveManager.instance.saveWorld.isSlotSaved[0] == false)
        {
            LevelManager.instance.currentLevelCompleted = 0;
        }
    }



    public void ShowMenu()//clicked load button on main menu to bring up load panel
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Button[] buttons = GetComponentsInChildren<Button>();
        buttons[0].Select();

    }


    public void OnButtonDelete()
    {
        SaveManager.instance.DeleteGameData(selectedSlot);
        UpdateButtons();
    }

    void Update()
    {


        if (player.GetButtonDown("select") && canvasGroup.interactable)
        {
            BackClicked();
            HideMenu();
        }
    }

    public void BackClicked()
    {
        OnBackClicked.Invoke();
    }

    public void HideMenu()
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }



}

