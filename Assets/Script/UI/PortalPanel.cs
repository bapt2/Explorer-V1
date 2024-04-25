using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortalPanel : MonoBehaviour
{

    public Button forestGenButton;
    public Button desertGenButton;
    public Button islandGenButton;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Hub")
        {
            forestGenButton.onClick.AddListener(Forest_Generation);
            desertGenButton.onClick.AddListener(Desert_Generation);
            islandGenButton.onClick.AddListener(Island_Generation);
        }
    }

    void Forest_Generation()
    {

        gameObject.SetActive(false);
        CameraController.instance.enabled = true;
        SceneManager.LoadScene("Procedural Generation_Forest");
    }

    void Desert_Generation()
    {
        gameObject.SetActive(false);
        CameraController.instance.enabled = true;
        SceneManager.LoadScene("Procedural Generation_Desert");
    }

    void Island_Generation()
    {
        gameObject.SetActive(false);
        CameraController.instance.enabled = true;
        SceneManager.LoadScene("Procedural Generation_Island");
    }
}
