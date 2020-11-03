using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Entities;
using UnityEngine;




public class SaveLevelManager : MonoBehaviour
{

    public List<SavePlayers> savePlayers = new List<SavePlayers>();


    public static SaveLevelManager instance = null;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);


    }



}











