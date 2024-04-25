using UnityEngine;

public class PickUp : MonoBehaviour
{
    public BaseItem item;
    public InsectItem insectItem;
    public FishItem fishItem;
    public PlantItem plantItem;
    public bool isInRange = false;

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.P) && isInRange)
        {
            if (insectItem != null)
                PickupInsect();

            else if (fishItem != null)
                PickupFish();

            else if (plantItem != null)
                PickupPlant();

            else if (item != null)
                PickupItem();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            InventoryManager.instance.DisplayPickUpMessage();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            InventoryManager.instance.ClosePickUpMessage();
        }
    }

    void PickupItem()
    {
        if (InventoryManager.instance.itemList.Count >= InventoryManager.instance.itemPlace)
        {
            StartCoroutine(InventoryManager.instance.ItemInventoryFull());
        }
        else
        {
            InventoryManager.instance.itemList.Add(item);
            InventoryManager.instance.ClosePickUpMessage();
            Destroy(gameObject);

        }
    }
    void PickupInsect()
    {
        if (InventoryManager.instance.insectItemList.Count >= InventoryManager.instance.insectPlace)
        {
            StartCoroutine(InventoryManager.instance.InsectInventoryFull());
        }
        else
        {
            InventoryManager.instance.insectItemList.Add(insectItem);
            InventoryManager.instance.ClosePickUpMessage();
            Destroy(gameObject);

        }
    }
    void PickupFish()
    {
        if (InventoryManager.instance.fishItemList.Count >= InventoryManager.instance.fishPlace)
        {
            StartCoroutine(InventoryManager.instance.FishInventoryFull());
        }
        else
        {
            InventoryManager.instance.fishItemList.Add(fishItem);
            InventoryManager.instance.ClosePickUpMessage();

            Destroy(gameObject);
        }
    }
    void PickupPlant()
    {
        if (InventoryManager.instance.plantItemList.Count >= InventoryManager.instance.plantPlace)
        {
            StartCoroutine(InventoryManager.instance.PlantInventoryFull());
        }
        else
        {
            InventoryManager.instance.plantItemList.Add(plantItem);
            InventoryManager.instance.ClosePickUpMessage();
            Destroy(gameObject);
        }
    }
}
