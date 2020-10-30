using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.AI;

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
    

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        buttons.ForEach((btn) => btn.onClick.AddListener(() =>
        PlayMenuClickSound(clickSound)));//shortcut instead of using inspector to add to each button

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

        if (deadParticleSystem)
        { 
            deadParticleSystem.Play(true);
        }

        if (audioSource)
        {
            audioSource.Play();
        }



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

    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DeadMenuComponent() {hide = true});
    }
}

