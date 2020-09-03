using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToggle : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        LevelOpen.toggleTrigger += Toggle;
    }
    void OnDisable()
    {
        LevelOpen.toggleTrigger -= Toggle;

    }

    [SerializeField]
    private int level;

    private void Toggle()
    {
        if ((LevelManager.instance.currentLevel + 1) == level)
        {
            GetComponent<Collider>().isTrigger = !GetComponent<Collider>().isTrigger;
        }
    }

}
