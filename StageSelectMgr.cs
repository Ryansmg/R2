using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectMgr : MonoBehaviour
{
    public static bool isStageSelection = false;
    
    const float fpFadeTime = 0.5f;
    const float uiFadeInX = 2f;

    public GameObject FrontPanel;
    public GameObject StartButton;
    static GameObject s_sb;
    static GameObject staticFP;
    static int checkn=0;
    static float fpFadeTimer = 0f;
    static bool isFading = false;
    static bool isFadingOut = false;
    public Image[] ssUiImages;
    public TMP_Text[] ssUiTexts;

    public GameObject planets;
    static GameObject staticp;
    public GameObject[] planetobjs;
    static GameObject[] s_planetobjs;
    public static string planetName;
    public static TMP_Text txt;
    public static void ResetSSMStaticVars()
    {
        isStageSelection = false;
        checkn = 0;
        fpFadeTimer = 0f;
        isFading = false;
        isFadingOut = false;
    }
    void Start()
    {
        staticFP = FrontPanel;
        staticp = planets;
        s_planetobjs = planetobjs;
        s_sb = StartButton;
    }
    void Update()
    {
        if (checkn > 1)
        {
            checkn = -2100000000;
            Debug.LogWarning("There are multiple StageSelectMgr(s), which can cause unexpected management actions.");
        }
        else checkn = 0;

        if(fpFadeTimer > 0 && isFading)
        {
            fpFadeTimer -= Time.deltaTime;
            Color fadeInColor = new(1, 1, 1, 1 - (fpFadeTimer - fpFadeTime * (uiFadeInX - 1)) / fpFadeTime);
            Color fadeInColorForUI = new(1, 1, 1, 1 - (fpFadeTimer / fpFadeTime / uiFadeInX));
            Color fadeOutColor = new(1, 1, 1, (fpFadeTimer - fpFadeTime*(uiFadeInX-1)) / fpFadeTime);
            staticFP.GetComponent<Image>().color = fadeInColor;
            foreach (GameObject obj in s_planetobjs)
                if (!Equals(obj.name.ToLower(), planetName)) obj.GetComponent<Image>().color = fadeOutColor;
            txt.color = fadeOutColor;
            foreach (Image image in ssUiImages) image.color = fadeInColorForUI;
            foreach (TMP_Text tmpt in ssUiTexts) tmpt.color = fadeInColorForUI;
        }
        else if(isFading)
        {
            isFading = false;
            isStageSelection = true;
        }
        else if(fpFadeTimer > 0 && isFadingOut)
        {
            fpFadeTimer -= Time.deltaTime;
            Color fadeInColor = new(1, 1, 1, 1 - fpFadeTimer / fpFadeTime);
            Color fadeOutColor = new(1, 1, 1, fpFadeTimer / fpFadeTime);
            staticFP.GetComponent<Image>().color = fadeOutColor;
            foreach (GameObject obj in s_planetobjs)
                if (!Equals(obj.name.ToLower(), planetName)) obj.GetComponent<Image>().color = fadeInColor;
            txt.color = fadeInColor;
            foreach (Image image in ssUiImages) image.color = fadeOutColor;
            foreach (TMP_Text tmpt in ssUiTexts) tmpt.color = fadeOutColor;
        }
        else if(isFadingOut)
        {
            isFadingOut = false;
            isStageSelection = false;
            staticFP.SetActive(false);
            s_sb.SetActive(false);
            staticp.GetComponent<Canvas>().sortingOrder = 0;
        }
        else fpFadeTimer = 0f;

        checkn++;
    }
    public static void ChangeToSS()
    {
        staticFP.SetActive(true);
        s_sb.SetActive(true);
        staticp.GetComponent<Canvas>().sortingOrder = 4;
        fpFadeTimer = fpFadeTime * uiFadeInX;
        isFading = true;
    }
    public void ResetSolar()
    {
        if (isFadingOut) return;
        ResetSSMStaticVars();
        isStageSelection = true;
        Planet.RSSV_wCamAdjust();
        fpFadeTimer = fpFadeTime;
        isFadingOut = true;
    }
}
