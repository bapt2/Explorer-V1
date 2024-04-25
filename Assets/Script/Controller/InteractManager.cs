using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractManager : MonoBehaviour
{
    public GameObject portalPanel;

    public GameObject player;
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PortalRange") && Input.GetKeyDown(KeyCode.E))
        {
            OpenPortalPanel();
            CameraController.instance.enabled = false;
        }
        else if (other.CompareTag("PortalRangeReturn") && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("Hub");
            PlayerController.instance.gameObject.transform.position = new Vector3(0f, 1f, 0f);
        }
    }


    void OpenPortalPanel()
    {
        portalPanel.SetActive(true);
        player.SetActive(true);
    }
}
