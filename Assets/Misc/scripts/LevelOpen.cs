using System;
using UnityEngine;
public class LevelOpen : MonoBehaviour
{
    public static event Action<string> showMessage;
    public static event Action toggleTrigger;


    private Animator animator;
    private Collider collider;
    [SerializeField]
    private int levelNumber;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip levelCompleteClip;

    public bool oneRemains;
    public bool borderOpening;


    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponentInParent<Collider>();
        audioSource = GetComponent<AudioSource>();
    }


    

    void Update()
    {

        int currentLevel = LevelManager.instance.currentLevel;

        int complete = LevelManager.instance.levelSettings[currentLevel].NpcDead + LevelManager.instance.levelSettings[currentLevel].NpcSaved;
        int targets = LevelManager.instance.levelSettings[currentLevel].potentialLevelTargets;

        //targets = 1;//delete

        if (complete >= targets & currentLevel == levelNumber && borderOpening == false)
        {
            

            //remove mesh or trigger animation
            animator.SetBool("open", true);
            collider.isTrigger = true;
            audioSource.PlayOneShot(levelCompleteClip);
            oneRemains = false;
            borderOpening = true;
            string message = "Border is opening";
            showMessage?.Invoke(message);
            toggleTrigger?.Invoke();


        }
        else if (complete == (targets - 1) & currentLevel == levelNumber && oneRemains == false)
        {
            oneRemains = true;
            borderOpening = false;
            string message = "One Little Roby still needs your help";
            showMessage?.Invoke(message);
        }




    }

}

