using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float sensitivity = 5f;

    void Update()
    {
        transform.localPosition += Vector3.up * Input.GetAxis("Mouse ScrollWheel") * sensitivity;
    }
}
