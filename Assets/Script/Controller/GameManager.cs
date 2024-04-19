using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public int amount;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !InventoryManager.instance.inventoryIsOpen)
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
}
