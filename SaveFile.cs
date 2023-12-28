using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SaveFile : MonoBehaviour
{
    public static Variables vars;
    public static bool doLoadOnStartInternal = false;
    public bool doLoadOnStart = false;
    public bool doLoadOnUpdate = false;
    public string manuallyChangeVars;
    public bool manualChangeLoad = false;
    public bool manualChangeSave = false;
    public bool manualSave = false;
    public bool openSaveFile = false;
    public bool openPersistentDataPath;
    public bool loaded = false;
    string savePath;
    // Start is called before the first frame update
    void Start()
    {
        if(doLoadOnStartInternal) Load();
        doLoadOnStart = doLoadOnStartInternal;
    }

    void Update()
    {
        if (manualChangeLoad)
        {
            manualChangeLoad = false;
            Load(manuallyChangeVars);
            UnityEngine.Debug.Log("Loaded.");
        }
        if (manualChangeSave)
        {
            manualChangeSave = false;
            Save(manuallyChangeVars);
        }
        if (manualSave)
        {
            manualSave = false;
            Save();
        }
        if (openSaveFile)
        {
            openSaveFile = false;
            Process.Start(savePath);
        }
        if (openPersistentDataPath)
        {
            openPersistentDataPath = false;
            Process.Start(Application.persistentDataPath);
        }
        if (doLoadOnUpdate)
        {
            doLoadOnUpdate = false;
            Load();
            UnityEngine.Debug.Log("Loaded.");
        }
        doLoadOnStartInternal = doLoadOnStart;

        try
        {
            if (loaded) vars.playTime += Time.deltaTime;
        } catch(NullReferenceException)
        {
            UnityEngine.Debug.LogError("\'vars\' is null. Reloading is required.");
            loaded = false;
        }
    }
    /// <summary>
    /// vars를 json 형태로 save0에 저장한다.
    /// </summary>
    public void Save()
    {
        string s = JsonConvert.SerializeObject(vars, Formatting.Indented);
        Save(s);
    }
    public void Save(string saveString)
    {
        UnityEngine.Debug.Log("Saved: " + saveString);
        File.WriteAllText(savePath, saveString);
        manuallyChangeVars = saveString;
    }
    public void Load()
    {
        if (savePath == null)
        {
            savePath = Application.persistentDataPath + "/save0";
            UnityEngine.Debug.Log(savePath);
            if (!File.Exists(savePath))
            {
                Load("");
                loaded = true;
                return;
            }
        } 
        string text = File.ReadAllText(savePath);
        Load(text);
        loaded = true;
    }
    public void Load(string saveFileString)
    {
        try
        {
            if (saveFileString.Equals("")) throw new ArgumentException();
            vars = JsonConvert.DeserializeObject<Variables>(saveFileString);
        }
        catch (Exception)
        {
            vars = new Variables(true);
            Save();
            saveFileString = JsonConvert.SerializeObject(vars, Formatting.Indented);
        }

        manuallyChangeVars = saveFileString;

    }
    public void VarsUpdated()
    {
        string s = JsonConvert.SerializeObject(vars, Formatting.Indented);
        UpdateMCV(s);
        if(vars.autoSave) Save(s);
    }
    public void UpdateMCV()
    {
        manuallyChangeVars = JsonConvert.SerializeObject(vars, Formatting.Indented);
    }
    public void UpdateMCV(string indentedSerializedObj)
    {
        manuallyChangeVars = indentedSerializedObj;
    }
}
