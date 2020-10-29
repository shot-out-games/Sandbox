using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Entities;
using UnityEngine;




public class SaveManager : MonoBehaviour
{

    public static SaveManager instance = null;

    public SaveData saveData = new SaveData();
    public SaveWorld saveWorld = new SaveWorld();

    public bool updateScore = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        saveWorld = LoadSaveWorld();
        saveData = LoadSaveData();
        DontDestroyOnLoad(gameObject);


    }

    public SaveWorld LoadSaveWorld()
    {
        SaveWorld sw;
        string path = Application.persistentDataPath + "/game.wor";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            sw = bf.Deserialize(file) as SaveWorld;
            if (sw.lastLoadedSlot == 0)
            {
                sw.lastLoadedSlot = 1;
            }
            file.Close();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            sw = new SaveWorld { lastLoadedSlot = 1 };
            bf.Serialize(file, sw);
            file.Close();
        }

        //Debug.Log("save world load res height "+saveWorld.screenResHeight);

        return sw;

    }


    public void SaveWorldSettings()
    {
        Debug.Log("save world");
        int slot = saveWorld.lastLoadedSlot;
        saveWorld.isSlotSaved[slot] = true && slot > 0; 
        string path = Application.persistentDataPath + "/game.wor";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, saveWorld);
        file.Close();

    }

    

    public SaveData LoadSaveData()
    {
        int slot = saveWorld.lastLoadedSlot;
        //string path = Application.persistentDataPath + "/savedGames" + slot.ToString() + ".sog";
        string path = Application.persistentDataPath + "/savedGames" + ".sog";
        if (File.Exists(path))
        {
            Debug.Log(path + " exists");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveData sd = bf.Deserialize(file) as SaveData;
            file.Close();
            return sd;

        }
        else
        {
            Debug.Log(path + " created");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path);
            SaveData sd = new SaveData();
            bf.Serialize(file, sd);
            file.Close();
            return sd;
        }
    }


    public void SaveGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        //string fileName = Application.persistentDataPath + "/savedGames" + (saveWorld.lastLoadedSlot).ToString() + ".sog";
        string fileName = Application.persistentDataPath + "/savedGames" + ".sog";
        FileStream file = File.Create(fileName);
        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void DeleteGameData(int slot)
    {
        saveWorld.isSlotSaved[slot] = false;
        int count = saveData.saveGames.Count;
        if (slot == 1 && count == 0)
        {
            saveData.saveGames.Add(new SaveGames());
        }

        saveData.saveGames[slot-1] = new SaveGames();



        SaveGameData();

        //BinaryFormatter bf = new BinaryFormatter();
        //string fileName = Application.persistentDataPath + "/savedGames" + slot.ToString() + ".sog";
        //if (File.Exists(fileName))
        //{
        //    File.Delete(fileName);
        //}

    }






    //public void LoadHighScoreData()
    //{
    //    SaveData sd = new SaveData();
    //    string path = Application.persistentDataPath + "/savedGames" + ".sog";
    //    if (File.Exists(path))
    //    {
    //        Debug.Log(path + " exists");
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file = File.Open(path, FileMode.Open);
    //        sd = bf.Deserialize(file) as SaveData;
    //        file.Close();

    //    }
    //    else
    //    {
    //        Debug.Log(path + " created");
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file = File.Create(path);
    //        sd = new SaveData();
    //        bf.Serialize(file, sd);
    //        file.Close();
    //    }

    //    saveData = sd;

    //}



    //public void SaveHighScoreData()
    //{
    //    BinaryFormatter bf = new BinaryFormatter();
    //    //string fileName = Application.persistentDataPath + "/savedGames" + (saveWorld.lastLoadedSlot).ToString() + ".sog";
    //    string fileName = Application.persistentDataPath + "/savedGames" + ".sog";
    //    FileStream file = File.Create(fileName);
    //    Debug.Log(Application.persistentDataPath);
    //    bf.Serialize(file, saveData);
    //    file.Close();



    //}


    public void DeleteHighScoreData()
    {
        int slot = saveWorld.lastLoadedSlot - 1;
        saveData.saveGames[slot].scoreList.Clear();
        Debug.Log("clear");
        //SaveHighScoreData();

    }

}











