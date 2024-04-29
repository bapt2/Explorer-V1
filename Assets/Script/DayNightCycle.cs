using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float sensitivity;
    public float orbitDamping;
    public Vector3 localRot;

    public int nightCount = 0;
    public int dayCount = 1;
    public int numDay = 1;

    public bool checkCycle = false;

    public bool isDay = true;
    public bool isNight = false;

    [SerializeField] Light sunLight;
    [SerializeField] Light moonLight;

    // Update is called once per frame
    void Update()
    {
        float unscaleTime = Time.unscaledTime;

        if (isNight)
            Night();
        if (isDay)
            Day();

        localRot.y = unscaleTime * sensitivity;

        Quaternion qt = Quaternion.Euler(localRot.y, 0f, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, qt, unscaleTime * orbitDamping);
        
        if (Mathf.Approximately(Mathf.FloorToInt(localRot.y / 360f), numDay))
        {
            numDay += 1;
        }

        if (Mathf.Approximately(Mathf.FloorToInt(localRot.y / 180f), nightCount + dayCount))
        {
            if (isNight)
            {
                isNight = false;
                isDay = true;
                nightCount += 1;
                return;
            }
            if (isDay)
            {
                isDay = false;
                isNight = true;
                dayCount += 1;
            }
        }
    }

    public void Day()
    { 
        if (sunLight.intensity < 1.5f)
            sunLight.intensity += 0.01f;
    }

    public void Night()
    {
        
        if (sunLight.intensity > 0)
            sunLight.intensity -= 0.01f;
    }
}
