using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public GameObject inventoryPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !InventoryManager.instance.inventoryIsOpen && !DataPersistanceManager.instance.isProcGenScene)
        {
            DataPersistanceManager.instance.SaveGame();
            SceneManager.LoadScene("Main Menu");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && InventoryManager.instance.inventoryIsOpen)
        {
            inventoryPanel.SetActive(false);
            InventoryManager.instance.inventoryIsOpen = false;
            PlayerController.instance.enabled = true;
            CameraController.instance.enabled = true;
        }
    }

    public void LoadData(GameData data)
    {
        if (DataPersistanceManager.instance.currentScene == "Hub")
            PlayerController.instance.transform.position = data.playerPosition;
    }

    public void SaveData(GameData data)
    {
        if (DataPersistanceManager.instance.currentScene == "Hub")
            data.playerPosition = PlayerController.instance.transform.position;
    }
}
