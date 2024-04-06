using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity;
    public float orbitDamping;
    public Transform player;
    Vector3 localRot;

    bool find = false;

    void Update()
    {
        if (!find)
        {
            player = FindObjectOfType<PlayerController>().transform;
            find = true;
        }
        else
        {
            transform.position = player.position;
            localRot.x += Input.GetAxis("Mouse X") * sensitivity;
            // inverse this if you want to inverse the camera control
            localRot.y -= Input.GetAxis("Mouse Y") * sensitivity;

            localRot.y = Mathf.Clamp(localRot.y, -30f, 80f);

            Quaternion qt = Quaternion.Euler(localRot.y, localRot.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.deltaTime * orbitDamping);
        }
    }
}
