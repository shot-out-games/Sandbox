using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class PlayerData
{
    public float[] position = new float[3];
    //public SkillTreeComponent skillTree;
    //public LevelCompleteComponent savedLevelComplete;
    //public DeadComponent savedDead;
    //public WinnerComponent savedWinner;
    public HealthComponent savedHealth;
    //public ControlBarComponent savedControl;
    public PlayerComponent savedPlayer;
    //public AttachWeaponComponent savedAttachWeapon;

}
