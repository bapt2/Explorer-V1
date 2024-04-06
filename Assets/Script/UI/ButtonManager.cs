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
    #endregion


    #region MainMenu
    public void StartButton()
    {
        mainMenuPanel.SetActive(false);
        saveMenuPanel.SetActive(true);
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

    public void Forest_Generation()
    {
        SceneManager.LoadScene("Procedural Generation_Forest");
    }

    public void Desert_Generation()
    {
        SceneManager.LoadScene("Procedural Generation_Desert");
    }
    
    public void Island_Generation()
    {
        SceneManager.LoadScene("Procedural Generation_Island");
    }
}
