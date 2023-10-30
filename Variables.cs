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
    public int[] puzzleStatus;
    public string[] puzzleName;
    public bool[] tutorialShown;
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
        puzzleName = new string[] { "tutorial0", "tutorial1", "tutorial2" };
        puzzleStatus = new int[] { Constant.PUZZLE_NEW, Constant.PUZZLE_NOT_OPENED, Constant.PUZZLE_HIDDEN };
        tutorialShown = new bool[] { false,  false };
    }
}