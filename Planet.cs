using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;
    public static string following = "";
    public float smoothTime;
    public float ss_smoothTime;
    static float curSmoothTime;
    const float scrollScale = -90f;
    const float camMouseMoveScale = 0.05f;
    const float ovalXScale = 1.1f;
    const float camXdiff = 1551;
    public static Vector2 camOffset = Vector2.zero;
    static bool scrollZoomAllowed = true;
    static bool planetClickAllowed = true;

    // orbiting
    public bool doOrbit = false;
    public float angle = 0f;
    public float radius;
    public float period = 2f;
    public float camZoom;
    public float sszoom;
    public float ssXOff;
    static Vector3 center;
    Dictionary<string, int> nameToIndex = new();
    bool angleReset = false;
    int planetIndex;
    const float defaultCamSize = 5700f;

    public GameObject whiteCircle;
    public GameObject planetInfo;

    public Camera cam;
    static Vector3 velocity = Vector3.zero;
    static Vector3 camTarget = new(camXdiff, 0, -10);
    static Vector2 targetSize = new(defaultCamSize, defaultCamSize);
    static Vector2 curSize = new(defaultCamSize, defaultCamSize);
    static Vector2 velocity2 = Vector2.zero;
    static Vector2 velocity3 = Vector2.zero;
    
    public static void ResetSolarStaticVars()
    {
        following = "";
        camOffset = Vector2.zero;
        scrollZoomAllowed = true;
        velocity = Vector3.zero;
        camTarget = new(camXdiff, 0, -10);
        targetSize = new(defaultCamSize, defaultCamSize);
        curSize = new(defaultCamSize, defaultCamSize);
        velocity2 = Vector2.zero;
        velocity3 = Vector2.zero;
        planetClickAllowed = true;
    }
    public static void RSSV_wCamAdjust()
    {
        following = "";
        camOffset = Vector2.zero;
        scrollZoomAllowed = true;
        camTarget = new(camXdiff, 0, -10);
        targetSize = new(defaultCamSize, defaultCamSize);
        planetClickAllowed = true;
        curSmoothTime = GameObject.Find("Sun").GetComponent<Planet>().smoothTime;
    }
    void Start()
    {
        StartPuzzle.isDebugStart = false;
        Application.targetFrameRate = 360;
        radius = transform.position.x - center.x;
        if (gameObject.name.Equals("Sun"))
        {
            GameObject.Find("Scripts").GetComponent<SaveFile>().Load();
            curSmoothTime = smoothTime;
        } else if(doOrbit)
        {
            try
            {
                GameObject path = GameObject.Find($"{gameObject.name}Path");
                List<Vector3> list = new();
                for (float angle = 0f; angle <= Mathf.PI * 2; angle += 0.005f)
                {
                    list.Add(new Vector3(Mathf.Cos(angle) * ovalXScale * radius, Mathf.Sin(angle) * radius, 0));
                }
                list.Add(new Vector3(ovalXScale*radius, 0, 0));
                path.GetComponent<LineRenderer>().positionCount = list.Count;
                path.GetComponent<LineRenderer>().SetPositions(list.ToArray());
                path.GetComponent<LineRenderer>().sortingOrder = -1;
            } catch (NullReferenceException)
            {
                Debug.LogError("Couldn't find path for planet: " + planetName);
            }
        }
        nameToIndex.Add("mercury", 0);
        nameToIndex.Add("venus", 1);
        nameToIndex.Add("earth", 2);
        nameToIndex.Add("mars", 3);
        nameToIndex.Add("jupiter", 4);
        nameToIndex.Add("saturn", 5);
        nameToIndex.Add("uranus", 6);
        nameToIndex.Add("neptune", 7);
        nameToIndex.Add("pluto", 8);
        nameToIndex.Add("mong", 9);
        nameToIndex.TryGetValue(planetName, out planetIndex);
        center = GameObject.Find("Sun").transform.position;
    }
    public void OnClick()
    {
        if (StageSelectMgr.isStageSelection) return;
        if (!planetClickAllowed) return;
        Debug.Log($"planet clicked: {planetName}");
        if (following.Equals(planetName))
        {
            camOffset = new(ssXOff, 0);
            scrollZoomAllowed = false;
            planetClickAllowed = false;
            targetSize = new(camZoom/sszoom, camZoom/sszoom);
            StageSelectMgr.planetName = planetName;
            StageSelectMgr.txt = gameObject.GetComponentInChildren<TMP_Text>();
            StageSelectMgr.ChangeToSS();
        }
        else
        {
            whiteCircle.SetActive(true);
            following = planetName;
            camTarget = transform.position;
            targetSize = new(camZoom, camZoom);
        }
    }
    public void OnTextClick()
    {
        if (StageSelectMgr.isStageSelection) return;
        if (!planetClickAllowed) return;
        Debug.Log($"text clicked: {planetName}");
        camTarget = GameObject.Find(planetName).transform.position;
        float camSize = GameObject.Find(planetName).GetComponent<Planet>().camZoom;
        following = planetName.ToLower();
        targetSize = new(camSize, camSize);
    }
    public void CancelClick()
    {
        if (StageSelectMgr.isStageSelection) return;
        if (!planetClickAllowed) return;
        Debug.Log($"canceled: {planetName}");
        following = "";
        camTarget = GameObject.Find("Sun").transform.position;
        camTarget += new Vector3(camXdiff, 0, 0);
        targetSize = new(defaultCamSize, defaultCamSize);
    }
    public void CursorEnter()
    {
        if(following.Equals(planetName)) whiteCircle.SetActive(true);
    }
    public void CursorExit()
    {
        if (following.Equals(planetName)) whiteCircle.SetActive(false);
    }
    void Update()
    {
        if(!angleReset)
        {
            angleReset = true;
            angle = SaveFile.vars.planetAngle[planetIndex];
        }
        if (gameObject.name.Equals("Sun"))
        {
            if (Input.mouseScrollDelta.y != 0 && scrollZoomAllowed)
            {
                camTarget.x += (cam.ScreenToWorldPoint(Input.mousePosition).x - cam.transform.position.x) * camMouseMoveScale;
                camTarget.y += (cam.ScreenToWorldPoint(Input.mousePosition).y - cam.transform.position.y) * camMouseMoveScale;
            }
            float tsize = Mathf.Max(targetSize.x + Input.mouseScrollDelta.y * scrollScale, 1f);
            if (!scrollZoomAllowed) tsize = Mathf.Max(targetSize.x, 1f);
            targetSize = new Vector2(tsize, tsize);
            Vector3 tar = new(camTarget.x + camOffset.x, camTarget.y + camOffset.y, -10);
            curSmoothTime = Vector2.SmoothDamp(new Vector2(curSmoothTime, 0), new Vector2(StageSelectMgr.isStageSelection ? ss_smoothTime : smoothTime, 0), ref velocity3, smoothTime).x;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, tar, ref velocity, curSmoothTime);
            curSize = Vector2.SmoothDamp(curSize, targetSize, ref velocity2, smoothTime);
            cam.orthographicSize = curSize.x;
        } else if(doOrbit)
        {
            try
            {
                angle += Time.deltaTime * Mathf.PI * 2 / period * SaveFile.vars.orbitSpeed;
            } 
            catch (NullReferenceException) { return; }
            angle %= Mathf.PI * 2;
            SaveFile.vars.planetAngle[planetIndex] = angle;
            transform.position = center + new Vector3(Mathf.Cos(angle) * ovalXScale, Mathf.Sin(angle), 0) * radius;
            if (following.Equals(planetName))
            {
                camTarget = transform.position;
                planetInfo.SetActive(true);
            } else
            {
                planetInfo.SetActive(false);
            }
        }
    }
}
