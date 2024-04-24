using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotsMenu : MonoBehaviour
{
    [SerializeField] private SaveSlot[] saveSlots;
    [SerializeField] private GameObject saveSlotPrefab;


    [SerializeField] GameObject createGameButton;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject createSavePanel;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

/*    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        //disable all button
        DisableButtons();

        //update the selected profile id to be used for data persistence
        DataPersistanceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        // create a new game which will initialize our data to a clean slate

        DataPersistanceManager.instance.NewGame();

        DataPersistanceManager.instance.SaveGame();

        createSavePanel.SetActive(false);

        //load the scene which will in turn save the game because of OnSceneUnloaded() in the DataPersistenceManeger
        SceneManager.LoadSceneAsync("Hub");
    }/

    /*private void Start()
    {
        ActivateMenu();
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DataPersistanceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
        ActivateMenu();
    }

    public void ActivateMenu()
    {
        //Load all existing profile
        Dictionary<string, GameData> profilesGameData = DataPersistanceManager.instance.GetAllProfilesGameData();
        foreach (SaveSlot saveSlot in saveSlots)
        {
            Debug.Log("data set");
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            
            
        }
    }*/

    public void DisableButtons()
    {
        createGameButton.SetActive(false);
        backButton.SetActive(false);
    }
}
