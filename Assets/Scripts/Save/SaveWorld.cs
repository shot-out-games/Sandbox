using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveWorld 
{
//    public float musicVolume { get; set; }
    //public float soundVolume { get; set; }
    public float musicVolume = 100;//inspector view
    public float soundVolume = 100;
    //public int lastLoadedSlot;
    public bool[] isSlotSaved = new bool[4];//0 unused 1-3 save slots

//    public int graphicsQuality { get; set; }
//    public int screenResWidth { get; set; }
    //public int screenResHeight { get; set; }
    //public bool isFullScreen { get; set; }


}



