using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Camera camera = Camera.main;
        transform.LookAt(camera.transform.position);
        transform.Rotate(90, 0, 0);
    }
}
