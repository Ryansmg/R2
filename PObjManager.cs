using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PObjManager : MonoBehaviour
{
    public PObj pObj;
    Vector3 velocity = Vector3.zero;
    public float smoothness;

    void Update()
    {
        try
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(pObj.x, pObj.y, 0), ref velocity, smoothness);
        } catch (NullReferenceException) { }
    }
}
