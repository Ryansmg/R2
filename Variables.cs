using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Variables
{
    public float playTime; //현실 플레이 시간
    public long gold;
    public bool isHard;
    public int time; //게임 내 시간, 아침/점심/저녁/밤

    //settings
    public bool autoSave; //playTime 이외의 값이 바뀌면 자동 저장한다.
    //settingsEnd

    //status
    /// <summary>
    /// {puzzleName, puzzleStatus}
    /// </summary>
    public List<KeyValuePair<string, int>> puzzles; 

    public List<bool> tutorialShown;
    //statusEnd

    public Variables()
    {
    }
    public Variables(bool createDefault)
    {
        if (!createDefault) throw new Exception("Use Variables() instead.");
        Debug.Log("Creating default Variables..");
        playTime = 0;
        gold = 0;
        isHard = false;
        time = 0;
        puzzles = new()
        {
            new("tutorial0", Constant.PUZZLE_NEW),
            new("tutorial1", Constant.PUZZLE_NOT_OPENED),
            new("tutorial2", Constant.PUZZLE_HIDDEN)
        };
        tutorialShown = new()
        {
            false, false
        };
    }
}