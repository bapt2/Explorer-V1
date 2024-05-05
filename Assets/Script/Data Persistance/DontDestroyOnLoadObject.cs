using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadObject : MonoBehaviour
{
    public GameObject playerInterface;
    public GameObject portalInterface;
    public ItemSpriteDataBase itemSpriteDataBase;

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

    }

    public void DontActivateOnMainMenuScene()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            playerInterface.SetActive(false);
        else
            playerInterface.SetActive(true);
    }
}
