using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PTextManager : MonoBehaviour
{
    public string textType;
    public GameObject scripts;
    bool logErrorUTT = false;
    bool logErrorNRE = false;
    void Update()
    {
        if(!StartPuzzle.puzzleLoaded) { return; }
        try
        {
            TMP_Text text = gameObject.GetComponent<TMP_Text>();
            switch (textType)
            {
                case "oxygen":
                    int oxygenNow = scripts.GetComponent<StartPuzzle>().player.GetComponent<Player>().oxygen;
                    int maxOxygen = scripts.GetComponent<StartPuzzle>().player.GetComponent<Player>().puzzle.oxygen;
                    text.text = $"{oxygenNow}/{maxOxygen}";
                    break;
                case "hp":
                    float hpNow = scripts.GetComponent<StartPuzzle>().player.GetComponent<Player>().hp;
                    float maxHp = scripts.GetComponent<StartPuzzle>().player.GetComponent<Player>().puzzle.hp;
                    text.text = $"{hpNow:0.0}/{maxHp:0.0}";
                    break;
                default:
                    if (!logErrorUTT) Debug.LogError("Unsupported text type.");
                    logErrorUTT = true;
                    break;
            }
        } catch (NullReferenceException)
        {
            if (!logErrorNRE)
            {
                Debug.LogError("Waiting for restart...");
                logErrorNRE = true;
            }
        } catch (UnassignedReferenceException)
        {
            if (!logErrorNRE)
            {
                Debug.LogError("Waiting for restart...");
                logErrorNRE = true;
            }
        }
    }
}
