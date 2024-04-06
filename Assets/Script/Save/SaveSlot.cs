using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private TextMeshProUGUI worldName;


    [Header("ClearDataButton")]
    [SerializeField] GameObject deleteWarningPanel;

    GameObject NewWorldButton;

    SaveManager saveManager;

    public void SetData(GameData data)
    {
        worldName.text = "World Name: " + data.worldName;
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    private void Start()
    {
        NewWorldButton = transform.parent.parent.GetChild(3).gameObject;
    }

    public void StartGame()
    {
        saveManager.OnContinueGameClick();
    }

    public void DeleteWarning()
    {
        
        deleteWarningPanel.SetActive(true);
        NewWorldButton.SetActive(false);
    }

    public void CloseWarning()
    {
        deleteWarningPanel.SetActive(false);
        NewWorldButton.SetActive(true);
    }

    public void DeleteSave()
    {

        Destroy(gameObject);
        Debug.Log("test");
        CloseWarning();
    }


}
