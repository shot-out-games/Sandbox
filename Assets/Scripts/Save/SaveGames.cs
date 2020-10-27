using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SaveGames
{
    public List<SaveEnemies> saveEnemies = new List<SaveEnemies>();
    public List<SavePlayers> savePlayers = new List<SavePlayers>();
    public List<SaveNpc> saveNpc = new List<SaveNpc>();
    public List<LevelSettings> saveLevelData = new List<LevelSettings>();

    public List<WeaponItemComponent> saveWeapons = new List<WeaponItemComponent>();
    public List<PowerItemComponent> savePowerItems = new List<PowerItemComponent>();


    public int currentLevel;
    public LevelCompleteMenuComponent savedLevelWorld;
    public DeadMenuComponent savedDeadWorld;
    public WinnerMenuComponent savedWinnerWorld;
    public int playersDead;
    public int enemiesDead;
    public int NpcDead;
    public int playersSaved;
    public int enemiesSaved;
    public int NpcSaved;


    //this project only
    //public List<HoleComponent> savedHoles = new List<HoleComponent>();





}
