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
        slotText.text = sw.isSlotSaved[1] ? "CONTINUE " : "New Game";
        slotTextHighlighted.text = sw.isSlotSaved[1] ? "CONTINUE " : "New Game";
    }

    public void OnLoadSlotClicked(int slot)
    {
        selectedSlot = slot;
    }

    public void OnLoadOptionClicked()
    {
        loadButton.interactable = false;
        SaveManager.instance.LoadSaveWorld();
        SaveManager.instance.saveWorld.lastLoadedSlot = selectedSlot;
        SaveManager.instance.LoadSaveData();
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
        Debug.Log("s " + selectedSlot);
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

