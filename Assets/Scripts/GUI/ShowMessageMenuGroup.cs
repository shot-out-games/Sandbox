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


public struct ShowMessageMenuComponent : IComponentData
{

}


public class ShowMessageMenuGroup : MonoBehaviour, IConvertGameObjectToEntity
{
    private EntityManager manager;
    public Entity entity;
    public AudioSource audioSource;
    private List<Button> buttons;
    //public AudioClip clickSound;
    public EventSystem eventSystem;
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Button defaultButton;
    public float showTimeLength = 2.1f;
    private float showTimer = 0f;
    bool startShowTimer;
    [SerializeField] private TextMeshProUGUI message;
    public string messageString;
    [HideInInspector]
    public bool showOnce;
    public AudioClip voiceClip1;
    public AudioClip voiceClip2;
    public AudioClip voiceClip3;




    private void OnEnable()
    {
        //LevelOpen.showMessage += SetupMessage;

    }


    private void OnDisable()
    {
        //LevelOpen.showMessage -= SetupMessage;
    }


    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        //buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        //buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        //PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button
        startShowTimer = true;

    }


    private void SetupMessage(string _message)
    {
        messageString = _message;
        ShowMenu();
    }

    void Update()
    {
        if (startShowTimer)
        {
            showTimer += Time.deltaTime;
            if (showTimer > showTimeLength)
            {
                showTimer = 0;
                startShowTimer = false;
                showOnce = false;
                HideMenu();
            }
        }
    }




    public void ShowMenu()
    {
        message.text = messageString;
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

    //void PlayMenuClickSound(AudioClip clip)
    //{
       // audioSource.PlayOneShot(clip);

    //}

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        this.entity = entity;
        manager = dstManager;

        dstManager.AddComponentData(entity, new ShowMessageMenuComponent()
        {

        });
    }
}

