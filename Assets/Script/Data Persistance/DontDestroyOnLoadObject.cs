using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadObject : MonoBehaviour
{
    public GameObject playerInterFace;

    public static DontDestroyOnLoadObject instance { get; private set; }

    private void Awake()
    {


        if (instance != null)
        {
            Debug.LogWarning("More than one DontDestroyOnLoadObject in the scene. Destroying the newest one");
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().name == "Main Menu")
            playerInterFace.SetActive(false);
        else
            playerInterFace.SetActive(true);
    }
}
