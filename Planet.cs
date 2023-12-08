using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;
    static string following = "";
    public float smoothTime;
    const float scrollScale = -90f;
    const float camMouseMoveScale = 0.05f;
    const float ovalXScale = 1.04f;
    const float camXdiff = 1551;

    // orbiting
    public bool doOrbit = false;
    public float angle = 0f;
    public float radius;
    public float period = 2f;
    static Vector3 center;
    Dictionary<string, int> nameToIndex = new();
    bool angleReset = false;
    int planetIndex;
    const float defaultCamSize = 5400f;

    public Camera cam;
    static Vector3 velocity = Vector3.zero;
    static Vector3 camTarget = new(camXdiff, 0, -10);
    static Vector2 targetSize = new(defaultCamSize, defaultCamSize);
    static Vector2 curSize = new(defaultCamSize, defaultCamSize);
    static Vector2 velocity2 = Vector2.zero;

    void Start()
    {
        if (gameObject.name.Equals("Sun"))
        {
            GameObject.Find("SaveFile").GetComponent<SaveFile>().Load();
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
        radius = transform.position.x - center.x;
    }
    public void OnClick()
    {
        Debug.Log($"planet clicked: {planetName}");
        following = planetName;
        camTarget = transform.position;
        targetSize = new(1200f, 1200f);
    }
    public void OnTextClick()
    {
        Debug.Log($"text clicked: {planetName}");
        camTarget = GameObject.Find(planetName).transform.position;
        following = planetName.ToLower();
        targetSize = new(1200f, 1200f);
    }
    public void CancelClick()
    {
        Debug.Log($"canceled: {planetName}");
        following = "";
        camTarget = GameObject.Find("Sun").transform.position;
        camTarget += new Vector3(camXdiff, 0, 0);
        targetSize = new(defaultCamSize, defaultCamSize);
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
            if (Input.mouseScrollDelta.y != 0)
            {
                camTarget.x += (cam.ScreenToWorldPoint(Input.mousePosition).x - cam.transform.position.x) * camMouseMoveScale;
                camTarget.y += (cam.ScreenToWorldPoint(Input.mousePosition).y - cam.transform.position.y) * camMouseMoveScale;
            }
            float tsize = Mathf.Max(targetSize.x + Input.mouseScrollDelta.y * scrollScale, 1f);
            targetSize = new Vector2(tsize, tsize);
            Vector3 tar = new(camTarget.x, camTarget.y, -10);
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, tar, ref velocity, smoothTime);
            curSize = Vector2.SmoothDamp(curSize, targetSize, ref velocity2, smoothTime);
            cam.orthographicSize = curSize.x;
        } else if(doOrbit)
        {
            angle += Time.deltaTime * Mathf.PI * 2 / period * SaveFile.vars.orbitSpeed;
            angle %= Mathf.PI * 2;
            SaveFile.vars.planetAngle[planetIndex] = angle;
            transform.position = center + new Vector3(Mathf.Cos(angle) * ovalXScale, Mathf.Sin(angle), 0) * radius;
            if (following.Equals(planetName)) camTarget = transform.position;
        }
    }
}
