using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject inventoryPanel;
    public int amount;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inventoryManager.inventoryIsOpen)
        {
            DataPersistanceManager.instance.SaveGame();
            SceneManager.LoadScene("Main Menu");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && inventoryManager.inventoryIsOpen)
        {
            inventoryPanel.SetActive(false);
            PlayerController.instance.enabled = true;
            CameraController.instance.enabled = true;
        }
    }
}
