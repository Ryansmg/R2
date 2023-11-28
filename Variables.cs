using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Variables
{
    public float playTime; //���� �÷��� �ð�
    public long gold;
    public bool isHard;
    public int time; //���� �� �ð�, ��ħ/����/����/��

    //settings
    public bool autoSave; //playTime �̿��� ���� �ٲ�� �ڵ� �����Ѵ�.
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