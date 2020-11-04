using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SaveGames
{
    public List<SaveEnemies> saveEnemies = new List<SaveEnemies>();
    public List<SavePlayers> savePlayers = new List<SavePlayers>();
    public List<LevelSettings> saveLevelData = new List<LevelSettings>();
    public List<float> scoreList = new List<float>();



    public int currentLevel;


    public SaveGames()
    {
        //savePlayers.Add(new SavePlayers());
        //savePlayers.Add(new SavePlayers());
        //savePlayers.Add(new SavePlayers());
        //saveEnemies.Add(new SaveEnemies());
        //saveEnemies.Add(new SaveEnemies());
        //saveEnemies.Add(new SaveEnemies());
    }

}
