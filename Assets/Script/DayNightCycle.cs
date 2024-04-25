using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float sensitivity;
    public float orbitDamping;
    Vector3 localRot;

    // Update is called once per frame
    void Update()
    {
        localRot.y = Time.unscaledTime * sensitivity;

        Quaternion qt = Quaternion.Euler(localRot.y, 0f, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.unscaledTime * orbitDamping);
    }

    
}
