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

    //planet orbit
    public float[] planetAngle = new float[9];
    public float orbitSpeed;

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
        orbitSpeed = 10f;
        float[] firstPa = { 3.20318f, 3.31926775f, 1.52970839f, 
            3.7813437f, 0.9779366f, 3.182678f, 3.70370388f, 5.114613f, 0.0f};
        planetAngle = firstPa;
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