using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractManager : MonoBehaviour
{ 
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PortalRange") && Input.GetKeyDown(KeyCode.E))
            OpenPortalPanel();
        
        else if (other.CompareTag("PortalRangeReturn") && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Hub");
            PlayerController.instance.gameObject.transform.position = new Vector3(0f, 1f, 0f);
        }
    }


    void OpenPortalPanel()
    {
        DontDestroyOnLoadObject.instance.portalInterface.SetActive(true);

        CameraController.instance.enabled = false;
    }
}
