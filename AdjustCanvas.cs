using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustCanvas : MonoBehaviour
{
    void Update()
    {
        if (Screen.width <= Screen.height * 3 / 1.9f)
            gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
        else gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
    }
}
