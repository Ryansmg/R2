using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    public bool restartScene = false;
    public GameObject fadePanel;
    const float fadeInSec = 0.75f; //deactivate panel
    const float fadeOutSec = 0.75f; // activate panel
    static float fadeTimer = 0f;
    static string changeSceneName;
    static bool isFading = false;
    bool ctp = false;
    string ctp0; bool ctp1;
    public void Update()
    {
        if(isFading)
        {
            fadePanel.SetActive(true);
            fadeTimer -= Time.deltaTime;
            fadePanel.GetComponent<Image>().color = new Color(0, 0, 0, 1 - (fadeTimer / fadeOutSec));
            if (fadeTimer < 0)
            {
                fadeTimer = fadeInSec;
                isFading = false;
                if(ctp)
                {
                    StartPuzzle.puzzleName = ctp0;
                    StartPuzzle.isFirstPuzzle = ctp1;
                }
                SceneManager.LoadScene(changeSceneName);
            }
        }
        else if(fadeTimer > 0)
        {
            fadePanel.SetActive(true);
            fadePanel.GetComponent<Image>().color = new Color(0, 0, 0, fadeTimer / fadeInSec);
            fadeTimer -= Time.deltaTime;
            if (fadeTimer < 0)
            {
                fadeTimer = 0;
                fadePanel.SetActive(false);
            }
        }

        if (restartScene)
        {
            restartScene = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void ChangeToPuzzle(string puzzleName)
    {
        ctp = true;
        ctp0 = puzzleName;
        ctp1 = true;
        FadeChange("PuzzleScene");
    }
    public void ChangeToPuzzle(string puzzleName, bool isFirstPuzzle)
    {
        ctp = true;
        ctp0 = puzzleName;
        ctp1 = isFirstPuzzle;
        FadeChange("PuzzleScene");
    }
    public void Change(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void FadeChange(string sceneName)
    {
        changeSceneName = sceneName;
        isFading = true;
        fadeTimer = fadeOutSec;
    }
    public void ResetSolarAndFC(string sceneName)
    {
        StageSelectMgr.ResetSSMStaticVars();
        Planet.ResetSolarStaticVars();
        FadeChange(sceneName);
    }
    public void Exit()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.Quit();
            //AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            //activity.Call<bool>("moveTaskToBack", true);
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}
