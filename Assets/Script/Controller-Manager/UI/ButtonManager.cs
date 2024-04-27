using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    #region SerializedField
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject saveMenuPanel;
    [SerializeField] GameObject optionMenuPanel;
    [SerializeField] GameObject keyBindPanel;
    [SerializeField] GameObject videoSettingsPanel;
    [SerializeField] GameObject soundSettingsPanel;
    [SerializeField] GameObject maxSaveWarning;
    [SerializeField] GameObject newGamePanel;
    [SerializeField] GameObject deleteWarningPanel;
    [SerializeField] Button startButton;
    [SerializeField] Button createButton;
    #endregion

    public static ButtonManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of ButtonManager in the scene, destroy the newest one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        CheckSave();
    }

    #region MainMenu
    public void StartButton()
    {
       /* mainMenuPanel.SetActive(false);
        saveMenuPanel.SetActive(true);*/
    }

    public void OptionButton()
    {
        mainMenuPanel.SetActive(false);
        optionMenuPanel.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    #endregion

    #region Settings

    public void KeyBindButton()
    {
        if (videoSettingsPanel.activeInHierarchy == true)
        {
            videoSettingsPanel.SetActive(false);
            keyBindPanel.SetActive(true);
        }
        else if (soundSettingsPanel.activeInHierarchy == true)
        {
            soundSettingsPanel.SetActive(false);
            keyBindPanel.SetActive(true);
        }
        else if (keyBindPanel.activeInHierarchy == true)
        {
            keyBindPanel.SetActive(false);
        }
        else
        {
            keyBindPanel.SetActive(true);
        }
    }

    public void VideoSettingsButton()
    {
        if (keyBindPanel.activeInHierarchy == true)
        {
            keyBindPanel.SetActive(false);
            videoSettingsPanel.SetActive(true);
        }
        else if (soundSettingsPanel.activeInHierarchy == true)
        {
            soundSettingsPanel.SetActive(false);
            videoSettingsPanel.SetActive(true);

        }
        else if (videoSettingsPanel.activeInHierarchy == true)
        {
            videoSettingsPanel.SetActive(false);
        }
        else
        {
            videoSettingsPanel.SetActive(true);
        }
    }

    public void VolumeSettings()
    {
        if (keyBindPanel.activeInHierarchy == true)
        {
            keyBindPanel.SetActive(false);
            soundSettingsPanel.SetActive(true);
        }
        else if (videoSettingsPanel.activeInHierarchy == true)
        {
            videoSettingsPanel.SetActive(false);
            soundSettingsPanel.SetActive(true);
        }
        else if (soundSettingsPanel.activeInHierarchy == true)
        {
            soundSettingsPanel.SetActive(false);
        }
        else
        {
            soundSettingsPanel.SetActive(true);
        }
    }
    #endregion

    public void WorldCreationButton()
    {
        saveMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);
    }


    public void BackButton()
    {
        if (saveMenuPanel.activeInHierarchy == true)
        {
            if (maxSaveWarning.activeInHierarchy == true)
            {
                maxSaveWarning.SetActive(false);
                return;
            }
            else if (deleteWarningPanel.activeInHierarchy)
            {
                deleteWarningPanel.SetActive(false);
            }
            else
            {
                saveMenuPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
            }
        }
        else if (newGamePanel.activeInHierarchy == true)
        {
            newGamePanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else if (optionMenuPanel.activeInHierarchy == true)
        {
            if (keyBindPanel.activeInHierarchy == true)
            {
                keyBindPanel.SetActive(false);
            }
            else if (videoSettingsPanel.activeInHierarchy == true)
            {
                videoSettingsPanel.SetActive(false);
            }
            else if (soundSettingsPanel.activeInHierarchy == true)
            {
                soundSettingsPanel.SetActive(false);
            }
            optionMenuPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }


    #region GameOver Menu




    public void CheckSave()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" && DataPersistanceManager.instance.dataExist)
            startButton.onClick.AddListener(FindObjectOfType<DataPersistanceManager>().StartGameButton);

        else if (SceneManager.GetActiveScene().name == "Main Menu" && !DataPersistanceManager.instance.dataExist)
        {
            startButton.onClick.AddListener(WorldCreationButton);
            createButton.onClick.AddListener(DataPersistanceManager.instance.StartGameButton);
        }
    }
    #endregion
}
