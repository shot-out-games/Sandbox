using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.AI;
using TMPro;

[System.Serializable]
public struct DeadMenuComponent : IComponentData
{
    public bool hide;
}


public class DeadMenuGroup : MonoBehaviour, IConvertGameObjectToEntity      
{

    // Use this for initialization
    public AudioSource audioSource;
    private List<Button> buttons;
    public AudioClip clickSound;
    public EventSystem eventSystem;
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Button defaultButton;

    [SerializeField] private ParticleSystem deadParticleSystem;
    [SerializeField]
    private TextMeshProUGUI message;
    [SerializeField] private float showTimer = 3;


    [HideInInspector] public bool showMenu;
    [HideInInspector] public bool showScoreboard;
    [HideInInspector] public int score;
    [HideInInspector] public int rank;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button

    }


    void Update()
    {
        if (showMenu && showTimer >= 0)
        {
            showTimer -= Time.deltaTime;
            if (showTimer <= 0)
            {
                ShowMenu();
            }
        }
    }

    void ShowMenu()
    {

        canvasGroup.alpha = 1;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        if (defaultButton)
        {
            defaultButton.Select();
        }

        if (deadParticleSystem)
        {
            deadParticleSystem.Play(true);
        }

        if (audioSource)
        {
            audioSource.Play();
        }


        if (showScoreboard == false)
        {
            message.SetText("Game Over");
        }
        else
        {
            message.SetText("SCORE: " + score + " RANK:  " + rank);
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

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DeadMenuComponent() {hide = true});
    }
}

