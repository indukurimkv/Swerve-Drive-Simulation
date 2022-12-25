using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform followCam;
    [SerializeField] Vector3 offset;


    // Update is called once per frame
    void Update()
    {
        followCam.position = transform.position + offset;
    }
}
