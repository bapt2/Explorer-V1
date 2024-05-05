using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    public GameObject inventoryPanel;
    public bool inventoryIsOpen = false;

    public GameObject slotPrefab;

    public GameObject itemPanel;
    public GameObject insectPanel;
    public GameObject fishPanel;
    public GameObject plantPanel;

    public GameObject itemContent;
    public GameObject insectContent;
    public GameObject fishContent;
    public GameObject plantContent;

    public GameObject itemFull;
    public GameObject insectFull;
    public GameObject fishFull;
    public GameObject plantFull;
    public GameObject pickUpMessage;

    public int itemPlace = 20;
    public int insectPlace = 10;
    public int fishPlace = 10;
    public int plantPlace = 20;

    public List<BaseItem> itemList;
    public List<BaseItem> insectItemList;
    public List<BaseItem> fishItemList;
    public List<BaseItem> plantItemList;

    GameObject _slotPrefab;
    ItemSlot itemslot;

    public static InventoryManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of inventory Manager in the scene, Destroying the newest one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !inventoryIsOpen && SceneManager.GetActiveScene().name != "Main Menu" && !PlayerStatsManager.instance.die)
        {
            inventoryPanel.SetActive(true);
            inventoryIsOpen = true;
            PlayerController.instance.rb.velocity = Vector3.zero;
            PlayerController.instance.enabled = false;
            CameraController.instance.enabled = false;
            DisplayItemPanel();
        }
        else if (Input.GetKeyDown(KeyCode.I) && inventoryIsOpen && SceneManager.GetActiveScene().name != "Main Menu" && !PlayerStatsManager.instance.die)
        {
            inventoryPanel.SetActive(false);
            inventoryIsOpen = false;
            PlayerController.instance.enabled = true;
            CameraController.instance.enabled = true;
        }
    }

    public void DisplayItemPanel()
    {
        itemPanel.SetActive(true);
        insectPanel.SetActive(false);
        fishPanel.SetActive(false);
        plantPanel.SetActive(false);
        CreateItemSlot(itemList, itemPlace, itemContent, itemPanel);
    }

    public void DisplayInsectPanel()
    {
        itemPanel.SetActive(false);
        insectPanel.SetActive(true);
        fishPanel.SetActive(false);
        plantPanel.SetActive(false);
        CreateItemSlot(insectItemList, insectPlace, insectContent, insectPanel);

    }

    public void DisplayFishPanel()
    {
        itemPanel.SetActive(false);
        insectPanel.SetActive(false);
        fishPanel.SetActive(true);
        plantPanel.SetActive(false);
        CreateItemSlot(fishItemList, fishPlace, fishContent, fishPanel);

    }

    public void DisplayPlantPanel()
    {
        itemPanel.SetActive(false);
        insectPanel.SetActive(false);
        fishPanel.SetActive(false);
        plantPanel.SetActive(true);
        CreateItemSlot(plantItemList, plantPlace, plantContent, plantPanel);

    }

    public void CreateItemSlot(List<BaseItem> currentList, int maxPlaces, GameObject currentContent, GameObject currentPanel)
    {

            foreach (Transform child in currentContent.transform)
            {
                if (currentPanel != null)
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < maxPlaces; i++)
            {
                _slotPrefab = null;
                if (i <= currentList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(currentContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = currentList[i].itemName;
                    sprite.sprite = DontDestroyOnLoadObject.instance.itemSpriteDataBase.CheckSpriteByID(currentList[i].spriteID);
                }
                else if (i > currentList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(currentContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = null;
                    sprite.sprite = null;
                }
            }
        }
    
    public IEnumerator ItemInventoryFull(GameObject objectMessage)
    {
        objectMessage.SetActive(true);
        yield return new WaitForSeconds(3f);
        objectMessage.SetActive(false);
    }
    
    #region pick up message
    public void DisplayPickUpMessage()
    {
        pickUpMessage.SetActive(true);
    }
    
    public void ClosePickUpMessage()
    {
        pickUpMessage.SetActive(false);
    }
    #endregion

    public void LoadData(GameData data)
    {
        itemPlace = data.itemPlace;
        insectPlace = data.insectPlace;
        fishPlace = data.fishPlace;
        plantPlace = data.plantPlace;


        itemList = new();
        insectItemList = new();
        fishItemList = new();
        plantItemList = new();

        for (int item = 0; item < data.itemList.Count; item++)
        {
            itemList.Add(ScriptableObject.CreateInstance<BaseItem>());
            itemList[item].itemType = data.itemList[item].itemType;
            itemList[item].itemName = data.itemList[item].itemName;
            itemList[item].spriteID = data.itemList[item].spriteID;
        }

        for (int item = 0; item < data.insectItemList.Count; item++)
        {
            insectItemList.Add(ScriptableObject.CreateInstance<BaseItem>());
            insectItemList[item].itemType = data.insectItemList[item].itemType;
            insectItemList[item].itemName = data.insectItemList[item].itemName;
            insectItemList[item].spriteID = data.insectItemList[item].spriteID;
        }

        for (int item = 0; item < data.fishItemList.Count; item++)
        {
            fishItemList.Add(ScriptableObject.CreateInstance<BaseItem>());
            fishItemList[item].itemType = data.fishItemList[item].itemType;
            fishItemList[item].itemName = data.fishItemList[item].itemName;
            fishItemList[item].spriteID = data.fishItemList[item].spriteID;
        }

        for (int item = 0; item < data.plantItemList.Count; item++)
        {
            plantItemList.Add(ScriptableObject.CreateInstance<BaseItem>());
            plantItemList[item].itemType = data.plantItemList[item].itemType;
            plantItemList[item].itemName = data.plantItemList[item].itemName;
            plantItemList[item].spriteID = data.plantItemList[item].spriteID;
        }
    }

    public void SaveData(GameData data)
    {
        data.itemPlace = itemPlace;
        data.insectPlace = insectPlace;
        data.fishPlace = fishPlace;
        data.plantPlace = plantPlace;

        data.itemList = new();
        data.insectItemList = new();
        data.fishItemList = new();
        data.plantItemList = new();

        for (int item = 0; item < itemList.Count; item++)
        {
            data.itemList.Add(new AttributesItemData());
            data.itemList[item].itemType = itemList[item].itemType;
            data.itemList[item].itemName = itemList[item].itemName;
            data.itemList[item].spriteID = plantItemList[item].spriteID;
        }

        for (int item = 0; item < insectItemList.Count; item++)
        {
            data.insectItemList.Add(new AttributesItemData());
            data.insectItemList[item].itemType = insectItemList[item].itemType;
            data.insectItemList[item].itemName = insectItemList[item].itemName;
            data.insectItemList[item].spriteID = insectItemList[item].spriteID;
        }

        for (int item = 0; item < fishItemList.Count; item++)
        {
            data.fishItemList.Add(new AttributesItemData());
            data.fishItemList[item].itemType = fishItemList[item].itemType;
            data.fishItemList[item].itemName = fishItemList[item].itemName;
            data.fishItemList[item].spriteID = fishItemList[item].spriteID;
        }

        for (int item = 0; item < plantItemList.Count; item++)
        {
            data.plantItemList.Add(new AttributesItemData());
            data.plantItemList[item].itemType = plantItemList[item].itemType;
            data.plantItemList[item].itemName = plantItemList[item].itemName;
            data.plantItemList[item].spriteID = plantItemList[item].spriteID;
        }
    }
}
