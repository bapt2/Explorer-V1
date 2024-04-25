using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public List<BaseItem> itemList = new();
    public List<InsectItem> insectItemList = new();
    public List<FishItem> fishItemList = new();
    public List<PlantItem> plantItemList = new();

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
        if (Input.GetKeyDown(KeyCode.I) && !inventoryIsOpen && SceneManager.GetActiveScene().name != "Main Menu")
        {
            inventoryPanel.SetActive(true);
            inventoryIsOpen = true;
            PlayerController.instance.rb.velocity = Vector3.zero;
            PlayerController.instance.enabled = false;
            CameraController.instance.enabled = false;
            DisplayItemPanel();
        }
        else if (Input.GetKeyDown(KeyCode.I) && inventoryIsOpen && SceneManager.GetActiveScene().name != "Main Menu")
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
        CreateItemSlot(itemList);
    }

    public void DisplayInsectPanel()
    {
        itemPanel.SetActive(false);
        insectPanel.SetActive(true);
        fishPanel.SetActive(false);
        plantPanel.SetActive(false);
        CreateItemSlot(null, insectItemList);

    }

    public void DisplayFishPanel()
    {
        itemPanel.SetActive(false);
        insectPanel.SetActive(false);
        fishPanel.SetActive(true);
        plantPanel.SetActive(false);
        CreateItemSlot(null, null, fishItemList);

    }

    public void DisplayPlantPanel()
    {
        itemPanel.SetActive(false);
        insectPanel.SetActive(false);
        fishPanel.SetActive(false);
        plantPanel.SetActive(true);
        CreateItemSlot(null, null, null, plantItemList);

    }

    public void CreateItemSlot(List<BaseItem> _itemList = null, List<InsectItem> _insectItemList = null, List<FishItem> _fishItemList = null, List<PlantItem> _plantItemList = null)
    {
        if (_itemList == null && _insectItemList == null && _fishItemList == null && _plantItemList == null)
        {
            Debug.LogWarning("At least 1 of the list must be asigned");
            return;
        }

        else if (_itemList != null)
        {
            foreach (Transform child in itemContent.transform)
            {
                if (itemPanel != null)
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < itemPlace; i++)
            {
                _slotPrefab = null;
                if (i <= _itemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(itemContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = itemList[i].itemName;
                    sprite.sprite = itemList[i].sprite;

                }
                else if (i > _itemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(itemContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = null;
                    sprite.sprite = null;
                }
            }
        }

        else if (_insectItemList != null)
        {
            foreach (Transform child in insectContent.transform)
            {
                if (insectPanel != null)
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < insectPlace; i++)
            {
                _slotPrefab = null;
                if (i <= _insectItemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(insectContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = insectItemList[i].itemName;
                    sprite.sprite = insectItemList[i].sprite;
                }
                else if (i > _insectItemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(insectContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = null;
                    sprite.sprite = null;
                }
            }
        }

        else if (_fishItemList != null)
        {
            foreach (Transform child in fishContent.transform)
            {
                if (fishPanel != null)
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < fishPlace; i++)
            {
                _slotPrefab = null;

                if (i <= _fishItemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(fishContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = fishItemList[i].itemName;
                    sprite.sprite = fishItemList[i].sprite;

                }
                else if (i > _fishItemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(fishContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = null;
                    sprite.sprite = null;
                }
            }
        }

        else if (_plantItemList != null)
        {
            foreach (Transform child in plantContent.transform)
            {
                if (plantPanel != null)
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < plantPlace; i++)
            {
                _slotPrefab = null;

                if (i <= _plantItemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(plantContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = plantItemList[i].itemName;
                    sprite.sprite = plantItemList[i].sprite;
                }
                else if (i > _plantItemList.Count - 1)
                {
                    _slotPrefab = Instantiate(slotPrefab, transform.position, transform.rotation);
                    _slotPrefab.transform.SetParent(plantContent.transform);
                    itemslot = _slotPrefab.GetComponent<ItemSlot>();

                    TextMeshProUGUI itemName = itemslot.itemSlotName;
                    Image sprite = itemslot.itemSlotIcon;

                    itemslot.slotIndex = i;
                    itemName.text = null;
                    sprite.sprite = null;
                }
            }
        }
    }

    #region full inventory


    public IEnumerator ItemInventoryFull()
    {
        itemFull.SetActive(true);
        yield return new WaitForSeconds(3f);
        itemFull.SetActive(false);
    }
    public IEnumerator InsectInventoryFull()
    {
        insectFull.SetActive(true);
        yield return new WaitForSeconds(3f);
        insectFull.SetActive(false);
    }
    public IEnumerator FishInventoryFull()
    {
        fishFull.SetActive(true);
        yield return new WaitForSeconds(3f);
        fishFull.SetActive(false);
    }
    public IEnumerator PlantInventoryFull()
    {
        plantFull.SetActive(true);
        yield return new WaitForSeconds(3f);
        plantFull.SetActive(false);
    }
    #endregion

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

        itemList = data.itemList;
        insectItemList = data.insectItemList;
        fishItemList = data.fishItemList;
        plantItemList = data.plantItemList;
    }

    public void SaveData(GameData data)
    {
        data.itemPlace = itemPlace;
        data.insectPlace = insectPlace;
        data.fishPlace = fishPlace;
        data.plantPlace = plantPlace;

        data.itemList = itemList;
        data.insectItemList = insectItemList;
        data.fishItemList = fishItemList;
        data.plantItemList = plantItemList;
    }
}
