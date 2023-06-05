using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
        yRotation += mouseX;
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        Vector3 position = rotation * offset + target.position;
        transform.rotation = rotation;
        transform.position = position;
    }
}