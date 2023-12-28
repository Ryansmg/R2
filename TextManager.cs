using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public string textType;
    bool logErrorUTT = false;
    bool logErrorNRE = false;
    long fpsSum = 0; Queue<int> fpsQueue = new();
    const int fpsQueueLength = 1000; const int fpsBigChangeLimit = 15;
    private void Start()
    {
        fpsSum = 0; fpsQueue = new Queue<int>();
    }
    void Update()
    {
        try
        {
            TMP_Text text = gameObject.GetComponent<TMP_Text>();
            switch (textType)
            {
                case "fps":
                    double fpsdouble = Math.Round(1.0 / Time.deltaTime);
                    fpsSum += (long)fpsdouble;
                    fpsQueue.Enqueue((int)fpsdouble);
                    int currentCount = fpsQueue.Count;
                    if (Math.Abs(fpsdouble - (fpsSum / fpsQueue.Count)) > fpsBigChangeLimit)
                        for (int i = 0; i < 12; i++)
                        {
                            fpsSum -= fpsQueue.Dequeue();
                            fpsQueue.Enqueue((int)fpsdouble);
                            fpsSum += (long)fpsdouble;
                        }
                    if (fpsQueue.Count == fpsQueueLength+1) fpsSum -= fpsQueue.Dequeue();
                    //text.text = "FPS: " + fpsSum/ fpsQueue.Count + "\nNow: " + (int)fpsdouble + $"\n({fpsQueue.Count})";
                    text.text = $"FPS: {fpsSum/fpsQueue.Count - fpsSum / fpsQueue.Count%5} ({(int)fpsdouble})";
                    break;
                case "planetname":
                    string phtxt = "exitingSSmode, placeholder text, report error if you see this";
                    string pn = Planet.following switch
                    {
                        "mercury" => "荐己",
                        "venus" => "陛己",
                        "earth" => "瘤备",
                        "mars" => "拳己",
                        "jupiter" => "格己",
                        "saturn" => "配己",
                        "uranus" => "玫空己",
                        "neptune" => "秦空己",
                        "pluto" => "疙空己",
                        "mong" => "???",
                        _ => phtxt
                    };
                    if(!pn.Equals(phtxt)) text.text = $"[{pn}]";
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
        }
    }
}
