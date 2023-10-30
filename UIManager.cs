using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public string type;

    public static bool wasDamaged = false;
    public float damagePanelTimer = 0f;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!StartPuzzle.puzzleLoaded) return; 
        switch (type)
        {
            case "damagePanel":
                if (wasDamaged)
                {
                    damagePanelTimer = 0.25f;
                }
                if(damagePanelTimer > 0f)
                {
                    damagePanelTimer -= Time.deltaTime;
                }
                gameObject.GetComponent<Image>().color = new Color(1, 0, 0, 55 * 4 * damagePanelTimer / 255);
                break;
            default: Debug.LogError("Wrong UIManager type."); break;
        }

        wasDamaged = false;
    }
}
