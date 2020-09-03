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


public struct InstructionsMenuComponent : IComponentData
{
    public bool hide;
    public int index;
}


public class InstructionsMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
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
    private float showTimeLength = 2.0f;
    private float showTimer = 0f;
    private bool startShowTimer;


    private float hideTime = 1.0f;
    private float hideTimer;
    private bool startHideTimer;


    [SerializeField]
    private TextMeshProUGUI currentInstruction;

    [SerializeField]
    private int instructionCount = 1;

    [SerializeField]
    private int totalInstructions = 2;



    void Start()
    {
        hideTimer = hideTime;

        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button
        startShowTimer = true;
        //currentInstruction.text = "Please Find the Weapon";
        float fontSize = 25;
        bool mobile = LevelManager.instance.mobile;
        if (mobile == true) fontSize = 75;
        currentInstruction.fontSize = fontSize;

        //currentInstruction.canvasRenderer.SetAlpha(1);

    }

    void Update()
    {
        if (manager == null) return;
        if (instructionCount > totalInstructions) return;

        if (startShowTimer)
        {
            bool mobile = LevelManager.instance.mobile;
            showTimer += Time.deltaTime;
            if (showTimer > showTimeLength)
            {
                instructionCount = instructionCount + 1;
                float fontSize = 25;
                if (mobile == true) fontSize = 75;
                if (instructionCount == 3)
                {
                    currentInstruction.text = "Escape the Doom.";
                    //currentInstruction.text = ".";
                    currentInstruction.fontSize = fontSize;
                }
                if (instructionCount == 4)
                {
                    currentInstruction.fontSize = fontSize;
                    //currentInstruction.text = "Please use Right Click to aim directly in front of you.";
                    currentInstruction.text = "Find the 5 cubes.";
                }
                if (instructionCount == 2)
                {
                    currentInstruction.fontSize = fontSize;
                    currentInstruction.text = "Cursed to shoot and destroy";

                }
                if (instructionCount == 5)
                {
                    currentInstruction.text = "Damage the soul ghosts and your spirit will be destroyed.";
                }
                //if (instructionCount == 6)
                //{
                //    currentInstruction.text = "Reverse shots are very powerful and activate triggers.";
                //}
                //if (instructionCount == 7)
                //{
                //    currentInstruction.text = "Your reverse powers are limited so keep an eye on the bar.";
                //}



                //if (instructionCount == 5)
                //{
                //    currentInstruction.fontSize = fontSize;
                //    currentInstruction.text = "Save all the little Robies to raise the wall to the next area.";
                //    if (mobile == true)
                //        currentInstruction.text = "Please touch Button (A) to attempt to send released Robies to  safety.";
                //}
                //if (instructionCount == 6)
                //{
                //    currentInstruction.fontSize = fontSize;
                //    currentInstruction.text = "Please all away from the flames.";
                //}
                //if (instructionCount == 7)
                //{
                //    currentInstruction.fontSize = fontSize;
                //    currentInstruction.text = "Only holes without flames are safe for all.";
                //}

                showTimer = 0;
                startShowTimer = false;
                startHideTimer = true;
                HideMenu();
            }
        }
        else if (startHideTimer)
        {

            hideTimer -= Time.deltaTime;
            if (hideTimer < 0 && instructionCount <= totalInstructions)
            {
                hideTimer = hideTime;
                startShowTimer = true;
                startHideTimer = false;
                ShowMenu();
            }
        }


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



    public void ShowMenu()
    {
        startShowTimer = true;
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
        Debug.Log("clip " + clip);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity, new InstructionsMenuComponent
        {
            hide = true
        });
    }
}

