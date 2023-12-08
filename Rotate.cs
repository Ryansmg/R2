using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float period;
    float eulerAngle = 0f;

    // Update is called once per frame
    void Update()
    {
        eulerAngle += (360 * Time.deltaTime / period);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, eulerAngle);
    }
}
