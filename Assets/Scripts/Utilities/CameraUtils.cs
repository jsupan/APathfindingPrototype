using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtils : MonoBehaviour
{
    void Start()
    {
        float aspectRatio = Camera.main.aspect;
        float camSize = Camera.main.orthographicSize;
        float correctPositionX = aspectRatio * camSize;
        Camera.main.transform.position = new Vector3(correctPositionX, camSize, -10);
    }
}
