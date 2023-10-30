using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public bool restartScene = false;
    public void Update()
    {
        if (restartScene)
        {
            restartScene = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
