using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject firstStartCanvas;
    public GameObject normalStartCanvas;
    public GameObject scripts;
    SaveFile saveFile;
    // Start is called before the first frame update
    void Start()
    {
        saveFile = scripts.GetComponent<SaveFile>();
        saveFile.Load();
        if (!SaveFile.vars.tutorialShown[0])
        {
            firstStartCanvas.SetActive(true);
            normalStartCanvas.SetActive(false);
            SaveFile.vars.tutorialShown[0] = true;
            saveFile.VarsUpdated();
        } else
        {
            firstStartCanvas.SetActive(false);
            normalStartCanvas.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
