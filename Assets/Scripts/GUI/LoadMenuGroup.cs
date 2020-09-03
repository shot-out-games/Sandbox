using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//mish mash
//not consistent using entity components for some and static level settings (easier) for others

public class LoadMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{
    //[SerializeField]
    //private Text[] slotTexts = new Text[3];
    private CanvasGroup canvasGroup;
    private InputController inputController;
    public UnityEvent OnBackClicked = new UnityEvent();
    private int selectedSlot = 0;
    private EntityManager manager;
    private Entity e;
    [SerializeField]
    Button loadButton;
    [SerializeField]
    Button deleteButton;



    void Start()
    {

        canvasGroup = GetComponent<CanvasGroup>();
        inputController = GetComponent<InputController>();

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (SaveManager.instance.saveData.saveGames.Count == 0) return;

        SaveWorld sw = SaveManager.instance.saveWorld;


        TextMeshProUGUI[] slotTexts = GetComponentsInChildren<TextMeshProUGUI>(); //3 button texts

        


        for (int i = 1; i <= 3; i++)
        {

            //slotTexts[i* 2-2].text = "";
            slotTexts[i * 2 - 2].text = sw.isSlotSaved[i] ? "GAME " + i : "New Game";
            slotTexts[i * 2 - 1].text = sw.isSlotSaved[i] ? "GAME " + i : "New Game";

            //Debug.Log("btn "+i + " " +  btn_txt + " " + slotTexts.Length);
        }
    }


    public void OnLoadSlotClicked(int slot)
    {
        selectedSlot = slot;
        //loadButton.gameObject.SetActive(true);
        //deleteButton.gameObject.SetActive(true);
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
        if (!inputController) return;

        if (inputController.buttonSelect_Pressed && canvasGroup.interactable)
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
        e = entity;
        manager = dstManager;
        //manager.AddComponentData(e, new LoadComponent { value = false });
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }


}

