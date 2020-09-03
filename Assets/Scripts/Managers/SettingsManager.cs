using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // Start is called before the first frame update

    private static SettingsManager instance;

    // Singleton pattern implementation
    public static SettingsManager Instance => SettingsManager.instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }



 }

