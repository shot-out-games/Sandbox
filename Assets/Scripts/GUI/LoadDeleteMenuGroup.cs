using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class LoadDeleteMenuGroup : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    private int selectedSlot = 0;
    [SerializeField]
    private Button defaultButton;
    [SerializeField] private EventSystem eventSystem;




    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnLoadSlot(int slot)
    {
        if(!loadButton || !deleteButton) return;
        selectedSlot = slot;
        bool isSlotSaved = SaveManager.instance.saveWorld.isSlotSaved[slot];
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        loadButton.Select();
        deleteButton.interactable = isSlotSaved;
        loadButton.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(isSlotSaved);

        Debug.Log("slot");
    }

    public void OnButtonDelete()
    {
        //if deleted slot is last loaded slot then reset
        if (SaveManager.instance.saveWorld.lastLoadedSlot == selectedSlot)
        {
            SaveManager.instance.saveWorld.lastLoadedSlot = 0;
        }

        OnLoadSlot(selectedSlot);
    }


    private void OnEnable()
    {
        //        PauseMenuGroup.OptionsClickedEvent += ShowMenu;
        GameInterface.HideMenuEvent += HideMenu;

    }

    private void OnDisable()
    {
        //PauseMenuGroup.OptionsClickedEvent -= ShowMenu;
        GameInterface.HideMenuEvent -= HideMenu;


    }





    public void ShowMenu()
    {
        Debug.Log("show");
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
